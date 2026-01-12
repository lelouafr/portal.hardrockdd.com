//using Microsoft.SharePoint.Client;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Security;
//using System.Text;
//using System.Threading.Tasks;

//namespace DB.Infrastructure.Services
//{
//    public class SharePointClient
//    {
//        //public SharePointClient(string site, string username, string pass)
//        //{
//        //    SecureString password = new SecureString();
//        //    pass.ToCharArray().ToList().ForEach(e => password.AppendChar(e));
//        //    ClientContext context = new ClientContext(site)
//        //    {
//        //        Credentials = new SharePointOnlineCredentials(username, password)
//        //    };

//        //    Context = context;
//        //}
//        //public SharePointClient(Uri site, string username, string pass)
//        //{
//        //    SecureString password = new SecureString();
//        //    pass.ToCharArray().ToList().ForEach(e => password.AppendChar(e));
//        //    ClientContext context = new ClientContext(site)
//        //    {
//        //        Credentials = new SharePointOnlineCredentials(username, password)
//        //    };
//        //    context.RequestTimeout = System.Threading.Timeout.Infinite;
//        //    Context = context;
//        //}

//        //public ClientContext Context { get; set; }
//    }

//    public static class SharePointExt
//    {
//        //public static List<Web> GetSites(this Web web)
//        //{
//        //    web.Context.Load(web, f => f.Webs);
//        //    web.Context.ExecuteQuery();
//        //    return web.Webs.ToList();
//        //}

//        //public static Web GetSharePointSite(this Web web, string siteName)
//        //{
//        //    if (SiteExists(web, siteName))
//        //    {
//        //        var url = string.Concat(web.Url, "/sites/", siteName);
//        //        using var context = new ClientContext(url)
//        //        {
//        //            Credentials = web.Context.Credentials,
//        //        };
//        //        var checkWeb = context.Web;
//        //        context.Load(checkWeb);
//        //        context.ExecuteQuery();
//        //        return checkWeb;
//        //    }
//        //    else
//        //    {
//        //        return null;
//        //    }

//        //}

//        //public static bool SiteExists(this Web web, string siteName)
//        //{
//        //    var url = string.Concat(web.Url, "/sites/", siteName);
//        //    using var context = new ClientContext(url)
//        //    {
//        //        Credentials = web.Context.Credentials,
//        //    };
//        //    var checkWeb = context.Web;
//        //    context.Load(checkWeb);
//        //    try
//        //    {
//        //        context.ExecuteQuery();
//        //        return true;
//        //    }
//        //    catch
//        //    {
//        //        return false;
//        //    }
//        //}

//        //public static List<List> GetDocumentLists(this Web web)
//        //{
//        //    web.Context.Load(web, f => f.Lists.Where(l => l.BaseTemplate == (int)ListTemplateType.DocumentLibrary));
//        //    web.Context.ExecuteQuery();
//        //    return web.Lists.ToList();
//        //}

//        //public static List GetList(this Web web, string library, bool autoCreate = false)
//        //{
//        //    //if (ListExists(web, library))
//        //    //{
//        //    //    var list = web.Lists.GetByTitle(library);
//        //    //    web.Context.Load(list, f => f);
//        //    //    web.Context.Load(list, f => f.RootFolder);
//        //    //    web.Context.ExecuteQuery();

//        //    //    return list;
//        //    //}

//        //    //else if (autoCreate)
//        //    //{
//        //    //    ListCreationInformation creationInfo = new ListCreationInformation();
//        //    //    creationInfo.Title = library;
//        //    //    creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;
//        //    //    List list = web.Lists.Add(creationInfo);
//        //    //    list.Description = library;
//        //    //    list.ListExperienceOptions = ListExperience.NewExperience;

//        //    //    list.Update();
//        //    //    //list = GetList(web, library);

//        //    //    var navigation = web.Navigation;
//        //    //    var topNavigationCollection = web.Navigation.TopNavigationBar;
//        //    //    web.Context.Load(list, f => f.DefaultViewUrl);
//        //    //    web.Context.ExecuteQuery();
//        //    //    var navItem = new NavigationNodeCreationInformation();
//        //    //    navItem.Title = library;
//        //    //    navItem.Url = list.RootFolder.ServerRelativeUrl;
//        //    //    navItem.AsLastNode = true;

//        //    //    navigation.QuickLaunch.Add(navItem);


//        //    //    web.Context.Load(navigation);
//        //    //    web.Context.ExecuteQuery();

//        //    //    topNavigationCollection.Add(navItem);
//        //    //    web.Context.Load(topNavigationCollection);
//        //    //    web.Context.ExecuteQuery();

//        //    //    return list;
//        //    //}
//        //    //return null;
//        //}

//        //private static bool ListExists(this Web web, string library)
//        //{
//        //    var list = web.Lists.GetByTitle(library);
//        //    web.Context.Load(list, f => f);
//        //    try
//        //    {
//        //        web.Context.ExecuteQuery();

//        //        return true;
//        //    }
//        //    catch (ServerUnauthorizedAccessException uae)
//        //    {
//        //        //Trace.WriteLine($"You are not allowed to access this folder");
//        //        throw;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        //Trace.WriteLine($"Could not find folder {url}");
//        //        return false;
//        //    }
//        //}

//        //public static Folder AddFolder(this Web web, Folder parentFolder, string FolderPath)
//        //{
//        //    var folderNames = FolderPath.Split(new char[] { '/', '\\'}, StringSplitOptions.RemoveEmptyEntries);
//        //    var listUrl = parentFolder.ServerRelativeUrl;
//        //    foreach (var folderName in folderNames)
//        //    {
//        //        string url = parentFolder.ServerRelativeUrl + "/" + folderName;
//        //        if (!web.FolderExists(url))
//        //        {
//        //            var prams = new ListItemUpdateParameters();
//        //            parentFolder.AddSubFolder(folderName, prams);
//        //            web.Context.ExecuteQuery();
//        //        }
//        //        web.Context.Load(parentFolder, f => f.Folders);
//        //        web.Context.ExecuteQuery();
//        //        parentFolder = parentFolder.Folders.FirstOrDefault(f => f.Name == folderName);
//        //    }
//        //    string folderUrl = listUrl + "/" + FolderPath;

//        //    var folder = web.GetFolderByServerRelativeUrl(folderUrl);
//        //    web.Context.Load(folder, f => f);
//        //    web.Context.ExecuteQuery();
//        //    return folder;
//        //}

//        //public static Folder GetFolder(this Web web, string library, string folderpath)
//        //{
//        //    folderpath = folderpath.Replace("\\", "/");
//        //    var list = GetList(web, library);
//        //    if (FolderExists(web, list, folderpath))
//        //    {
//        //        folderpath = folderpath.Replace("\\", "/");
//        //        string url = list.RootFolder.ServerRelativeUrl + "/" + folderpath;
                
//        //        var folder = web.GetFolderByServerRelativeUrl(url);
//        //        web.Context.Load(folder, f => f);
//        //        web.Context.ExecuteQuery();
//        //        return folder;
//        //    }
//        //    //Create Folder
//        //    return AddFolder(web, list.RootFolder, folderpath);
//        //}

//        //public static bool FolderExists(this Web web, List list, string folderName)
//        //{
//        //    string url = list.RootFolder.ServerRelativeUrl + "/" + folderName;
//        //    return FolderExists(web, url);
//        //}

//        //public static bool FolderExists(this Web web, Folder currentListFolder, string folderName)
//        //{
//        //    string url = currentListFolder.ServerRelativeUrl + "/" + folderName;
//        //    return FolderExists(web, url);
//        //}

//        //private static bool FolderExists(this Web web, string url)
//        //{
//        //    var folder = web.GetFolderByServerRelativeUrl(url);
//        //    web.Context.Load(folder, f => f.Exists);
//        //    try
//        //    {
//        //        web.Context.ExecuteQuery();

//        //        if (folder.Exists)
//        //        {
//        //            return true;
//        //        }
//        //        return false;
//        //    }
//        //    catch (ServerUnauthorizedAccessException uae)
//        //    {
//        //        Trace.WriteLine($"You are not allowed to access this folder");
//        //        throw;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Trace.WriteLine($"Could not find folder {url}");
//        //        return false;
//        //    }
//        //}

//        //public static File GetFile(this ClientContext context, Folder folder, string fileName)
//        //{
//        //    string url = folder.ServerRelativeUrl + "/" + fileName;
//        //    var file = context.Web.GetFileByServerRelativeUrl(url);
//        //    context.Load(file, f => f.ServerRelativeUrl);
//        //    context.ExecuteQuery();
//        //    return file;
//        //}

//        //public static byte[] GetFileData(this ClientContext context, Folder folder, string fileName)
//        //{
//        //    string url = folder.ServerRelativeUrl + "/" + fileName;
//        //    var file = context.Web.GetFileByServerRelativeUrl(url);
//        //    context.Load(file, f => f.ServerRelativeUrl);
//        //    context.ExecuteQuery();

//        //    var ms = new System.IO.MemoryStream();
//        //    var fileInfo = File.OpenBinaryDirect(context, file.ServerRelativeUrl);
//        //    fileInfo.Stream.CopyTo(ms);

//        //    return ms.ToArray();
//        //}

//        //public static bool FileExists(this Web web, List list, string fileName)
//        //{
//        //    string url = list.RootFolder.ServerRelativeUrl + "/" + fileName;
//        //    return FileExists(web, url);
//        //}

//        //public static bool FileExists(this Web web, Folder currentListFolder, string fileName)
//        //{
//        //    string url = currentListFolder.ServerRelativeUrl + "/" + fileName;
//        //    return FileExists(web, url);
//        //}

//        //private static bool FileExists(Web web, string url)
//        //{
//        //    var file = web.GetFileByServerRelativeUrl(url);
//        //    web.Context.Load(file, f => f.Exists);
//        //    try
//        //    {
//        //        web.Context.ExecuteQuery();

//        //        if (file.Exists)
//        //        {
//        //            return true;
//        //        }
//        //        return false;
//        //    }
//        //    catch (ServerUnauthorizedAccessException uae)
//        //    {
//        //        Trace.WriteLine($"You are not allowed to access this file");
//        //        throw;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Trace.WriteLine($"Could not find file {url}");
//        //        return false;
//        //    }
//        //}
//    }
//}
