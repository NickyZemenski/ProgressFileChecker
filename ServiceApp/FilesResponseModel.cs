using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class FilesResponseModel
    {
       
   
        public List<FileInfo> items { get; set; }
        public int currentPage { get; set; }
        public int totalItems { get; set; }
        public int itemsPerPage { get; set; }
        public List<Sort> sorting { get; set; }

     
 


    }
    public class Sort
    {
        public string sortField { get; set; }
        public string sortDirection { get; set; }
    }
    public class FileInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public int fileSize { get; set; }
        public DateTime uploadStamp { get; set; }
    }
}
