//using System.Collections.Generic;

//namespace portal.Models
//{
//    public class TableViewModel
//    {
//        public TableViewModel()
//        {
//            MaxPageNumber = 5;
//            Pages = new List<TablePaginationViewModel>();


//        }

//        public string TableId { get; set; }

//        public int CurrentPage { get; set; }

//        public int PageCount { get; set; }

//        public int PageSize { get; set; }

//        public int RecordCount { get; set; }

//        public int FilteredRecordCount { get; set; }

//        public string SearchString { get; set; }

//        public bool PrevEnabled
//        {
//            get
//            {
//                return CurrentPage == 1 ? false : true;
//            }
//        }

//        public bool NextEnabled
//        {
//            get
//            {
//                return PageCount == 1 || CurrentPage == PageCount ? false : true;
//            }
//        }

//        public string RecordSummary
//        {
//            get
//            {
//                var startRange = ((CurrentPage - 1) * PageSize) + 1;
//                var endRange = (CurrentPage * PageSize) > FilteredRecordCount ? FilteredRecordCount : (CurrentPage * PageSize);

//                // Showing @(((Model.CurrentPage - 1) * Model.PageSize) + 1) to @((Model.CurrentPage * (Model.PageSize))) of @Model.FilteredRecordCount entries
//                var result = "Showing ";
//                result += startRange.ToString(AppCultureInfo.CInfo());
//                result += " to ";
//                result += endRange.ToString(AppCultureInfo.CInfo());
//                result += " of ";
//                result += FilteredRecordCount.ToString(AppCultureInfo.CInfo());



//                return result;
//            }
//        }

//        public int MaxPageNumber { get; set; }

//        public List<TablePaginationViewModel> Pages { get; }

//        public void SetupPages()
//        {
//            var results = new List<TablePaginationViewModel>();
//            var tempMaxPage = PageCount >= MaxPageNumber ? MaxPageNumber : PageCount;



//            if (CurrentPage <= (MaxPageNumber - 1))
//            {
//                for (int i = 0; i < tempMaxPage; i++)
//                {
//                    results.Add(new TablePaginationViewModel()
//                    {
//                        PageNumber = (i + 1),
//                        PageLabel = (i + 1).ToString(AppCultureInfo.CInfo()),
//                        Disabled = false,
//                        Active = CurrentPage == (i + 1) ? true : false
//                    });
//                }

//                if (CurrentPage != PageCount)
//                {
//                    results.Add(new TablePaginationViewModel()
//                    {
//                        PageNumber = 0,
//                        PageLabel = "...",
//                        Disabled = true,
//                        Active = false
//                    });
//                    results.Add(new TablePaginationViewModel()
//                    {
//                        PageNumber = PageCount,
//                        PageLabel = (PageCount).ToString(AppCultureInfo.CInfo()),
//                        Disabled = false,
//                        Active = CurrentPage == PageCount ? true : false
//                    });

//                }
//            }
//            else if (CurrentPage >= PageCount - MaxPageNumber)
//            {
//                results.Add(new TablePaginationViewModel()
//                {
//                    PageNumber = 1,
//                    PageLabel = (1).ToString(AppCultureInfo.CInfo()),
//                    Disabled = false,
//                    Active = CurrentPage == 1 ? true : false
//                });
//                results.Add(new TablePaginationViewModel()
//                {
//                    PageNumber = 0,
//                    PageLabel = "...",
//                    Disabled = true,
//                    Active = false
//                });
//                for (int i = PageCount - MaxPageNumber; i < PageCount; i++)
//                {
//                    results.Add(new TablePaginationViewModel()
//                    {
//                        PageNumber = (i + 1),
//                        PageLabel = (i + 1).ToString(AppCultureInfo.CInfo()),
//                        Disabled = false,
//                        Active = CurrentPage == (i + 1) ? true : false
//                    });
//                }
//            }
//            else
//            {
//                results.Add(new TablePaginationViewModel()
//                {
//                    PageNumber = 1,
//                    PageLabel = (1).ToString(AppCultureInfo.CInfo()),
//                    Disabled = false,
//                    Active = CurrentPage == 1 ? true : false
//                });
//                results.Add(new TablePaginationViewModel()
//                {
//                    PageNumber = 0,
//                    PageLabel = "...",
//                    Disabled = true,
//                    Active = false
//                });

//                for (int i = CurrentPage - 2; i <= CurrentPage; i++)
//                {
//                    results.Add(new TablePaginationViewModel()
//                    {
//                        PageNumber = (i + 1),
//                        PageLabel = (i + 1).ToString(AppCultureInfo.CInfo()),
//                        Disabled = false,
//                        Active = CurrentPage == (i + 1) ? true : false
//                    });
//                }

//                results.Add(new TablePaginationViewModel()
//                {
//                    PageNumber = 0,
//                    PageLabel = "...",
//                    Disabled = true,
//                    Active = false
//                });
//                results.Add(new TablePaginationViewModel()
//                {
//                    PageNumber = PageCount,
//                    PageLabel = (PageCount).ToString(AppCultureInfo.CInfo()),
//                    Disabled = false,
//                    Active = CurrentPage == PageCount ? true : false
//                });
//            }
//            Pages.AddRange(results);
//        }
//    }

//    public class TablePaginationViewModel
//    {
//        public int PageNumber { get; set; }

//        public bool Active { get; set; }

//        public bool Disabled { get; set; }

//        public string PageLabel { get; set; }


//    }
//}