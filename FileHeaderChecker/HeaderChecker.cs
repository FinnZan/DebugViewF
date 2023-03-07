using FinnZan.Utilities;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileHeaderChecker
{
    internal class HeaderChecker
    {
        private string _root;

        public HeaderChecker(string root) 
        {
            _root= root;
        }
        public List<CodeFile> MissingFiles { get; private set; } = new List<CodeFile>();

        private string[] _ignoredFolders = { "obj", "AwccWinUI", "AwccWpfCore", "AgentService", "UnitTests", "ServiceSimulation" };        

        public void ScanAll()
        {
            var repo = new Repository(Path.Combine(_root, ".git"));

            var files = Directory.GetFiles(_root, "*.cs", SearchOption.AllDirectories).Where(x => !IsIgnored(x));

            MissingFiles = new List<CodeFile>();

            foreach (var f in files)
            {
                var rp = System.IO.Path.GetRelativePath(_root, f).Replace("\\", "/");
                var author = "";
                DateTimeOffset? start = null;
                DateTimeOffset? end = null; 

                try
                {
                    var history = repo.Commits.QueryBy(rp);
                    var first = history.FirstOrDefault();
                    author = first.Commit.Author.Name;
                    end = first.Commit.Author.When;
                    start = history.LastOrDefault().Commit.Author.When;              

                    CommonTools.Log($"[{rp}][{start} - {end}]");
                }
                catch (Exception ex)
                {
                    CommonTools.Log($"!!! [{rp}]");
                }

                try
                {
                    var cf = new CodeFile() { FullPath = f, Start = start, End = end, Author = author };
                    var lines = File.ReadAllLines(f);
                    if (lines.Length > 0)
                    {
                        if (!HasCorrectHeader(lines, cf))
                        {
                            MissingFiles.Add(cf);
                        }
                    }
                    else 
                    {
                        CommonTools.Log($"Empty file [{f}]!!!");
                    }
                }
                catch (Exception ex)
                {
                    CommonTools.HandleException(ex);
                }
            }

            MissingFiles.Sort((a, b) =>
            {
                return a.FullPath.CompareTo(b.FullPath);
            });

            repo.Dispose();
        }

        public void AddAll()
        {
            foreach (var f in MissingFiles)
            {
                try
                {
                    AddOrUpdateHeader(f);
                }
                catch (Exception ex)
                {
                    CommonTools.HandleException(ex);
                }
            }
        }

        public void AddOrUpdateHeader(CodeFile f)
        {
            try
            {
                var fn = System.IO.Path.GetFileName(f.FullPath);

                var header =
$@"//
// DELL PROPRIETARY INFORMATION
//
// This software contains the intellectual property of Dell Inc. 
// Use of this software and the intellectual property contained
// therein is expressly limited to the terms and conditions of
// the License Agreement under which it is provided by or
// on behalf of Dell Inc. or its subsidiaries.
//
// Copyright {f.GetYearString()} Dell Inc. or its subsidiaries. All Rights Reserved.
//
// DELL INC. MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY OF THE SOFTWARE, 
// EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT.  
// DELL SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A RESULT OF USING, 
// MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS DERIVATIVES.
//
";

                var lines = new List<string>(File.ReadAllLines(f.FullPath));

                var index = GetHeader(lines.ToArray()).Length;
                lines = lines.GetRange(index,lines.Count-index);

                lines.Insert(0, header);
                File.WriteAllLines(f.FullPath, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #region Private

        private List<string> GetAllGitFiles()
        {
            var ret = new List<string>();

            var repo = new Repository(Path.Combine(_root, ".git"));

            RepositoryStatus status = repo.RetrieveStatus(new StatusOptions() { IncludeUnaltered = true | false });

            foreach (var item in status) //This only lists altered files
            {
                ret.Add(item.FilePath);
            }

            repo.Dispose();

            return ret;
        }

        private bool HasCorrectHeader(string[] lines, CodeFile f)
        {            
            var header = GetHeader(lines);
      
            if (header.Length > 0)
            {
                if (f.Start.HasValue && f.End.HasValue && f.Start.Value.Year == f.End.Value.Year)
                {
                    return header.Any( x => x.Contains($"Copyright {f.GetYearString()}"));
                }
                else
                {
                    return header.Any(x => x.Contains($"Copyright {f.GetYearString()}"));
                }
            }
            else
            {
                return false;
            }
        }

        private string[] GetHeader(string[] lines)
        {
            var i = 0;

            // assuming the header is at the top and the first line after it is "using or namespace" 
            while (i < lines.Length 
                && !(lines[i].Trim().StartsWith("using") || lines[i].Trim().StartsWith("namespace"))) 
            {
                i++;   
            }

            if (i >= lines.Length)
            {
                return new string[0];
            }
            else
            {
                return new List<string>(lines).GetRange(0, i).ToArray();
            }
        }

        private bool IsIgnored(string path)
        {
            if (path.EndsWith(".g.cs") || path.EndsWith(".Designer.cs"))
            {
                return true;
            }

            try
            {
                var folder = System.IO.Path.GetDirectoryName(path);
                foreach (var f in _ignoredFolders)
                {
                    if (folder.Contains(@$"\{f}"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
            }

            return false;
        }

        #endregion
    }
}
