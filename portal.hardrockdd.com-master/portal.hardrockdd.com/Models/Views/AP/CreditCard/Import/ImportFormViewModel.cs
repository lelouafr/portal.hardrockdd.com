using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Models.Views.AP.CreditCard.Import
{
    public class ImportFormViewModel
    {
        public ImportFormViewModel()
        {

        }

        public ImportFormViewModel(DB.Infrastructure.ViewPointDB.Data.CreditImport import)
        {
            Co = import.Co;
            ImportId = import.ImportId;

            Info = new CreditImportViewModel(import);
            ZionLines = new ZionImportLinesViewModel(import);
            WexLines = new WexImportLinesViewModel()
            {
                ImportId = import.ImportId,
                Co = import.Co,
            };
        }

        public byte Co { get; set; }

        public int ImportId { get; set; }

        public CreditImportViewModel Info { get; set; }

        public ZionImportLinesViewModel ZionLines { get; set; }
        
        public WexImportLinesViewModel WexLines { get; set; }
    }
}