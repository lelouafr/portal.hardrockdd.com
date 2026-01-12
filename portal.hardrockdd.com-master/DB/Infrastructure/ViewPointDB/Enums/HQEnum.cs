namespace DB
{
    public enum BatchStatusEnum
    {
        Open = 0,
        ValidationInProcess = 1,
        ValidationWarnings = 25,
        ValidatedErros = 2,
        ValidationOK = 3,
        PostingInProgress = 4,
        PostingSuccessful = 5,
        Canceled = 6,

    }
    public enum ExplorerObjectTypeEnum
    {
        File,
        Folder,
    }
    public enum ExplorerViewTypeEnum
    {
        List,
        Thumbnail,
        Tree,
    }

    public enum HQAttachmentStorageEnum
    {
        DB,
        FileSystem,
        SharePoint,
        DBAndSharePoint
    }

}