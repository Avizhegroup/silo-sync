namespace Silo.Ui.Gate.BLL;

public class TagInfo
{
    // #define CONTENT_TYPE_INVALID        0
    // #define CONTENT_TYPE_EPC            1
    // #define CONTENT_TYPE_TID            2
    // #define CONTENT_TYPE_USER           3
    // #define CONTENT_TYPE_RSSI           4
    // #define CONTENT_TYPE_ANT            5
    // #define CONTENT_TYPE_ID             6
    // #define CONTENT_TYPE_IP             7


    private int id;

    public int Id
    {
        get { return id; }
        set { id = value; }
    }


    private int errCode;

    public int ErrCode
    {
        get { return errCode; }
        set { errCode = value; }
    }

    private UHFTAGInfo uhfTagInfo;

    public UHFTAGInfo UhfTagInfo
    {
        get { return uhfTagInfo; }
        set { uhfTagInfo = value; }
    }

}
