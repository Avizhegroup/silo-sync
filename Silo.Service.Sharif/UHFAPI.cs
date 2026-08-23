using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Silo.Service.Sharif.Dtos;

namespace Silo.Service.Sharif;

public class UHFAPI 
{



    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFSetTempVal(byte tempVal);
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFGetTempVal(byte[] tempVal);


    // [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    // private extern static int UHFSetIp(byte[] ip, byte[] port);
    //[DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    //private extern static int UHFGetIp(byte[] ip, byte[] port);

    /*
     * 函数功能：  获取本机 IP 和端口号
     * 输出参数：  ipbuf + postbuf， IP+端口号
                   mask:掩码，4字节
                   gate:网关，4字节
     * 返回值：   0:成功    其它：失败
     */
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFGetIp(byte[] ip, byte[] port, byte[] mask, byte[] gate);
    /*
     * 函数功能：  设置本机 IP 和端口号
     * 输入参数：  ipbuf： IP， 
                   postbuf：端口号
                   mask:掩码，4字节
                   gate：网关，4字节

     * 返回值：   0:成功    其它：失败
     */
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFSetIp(byte[] ipbuf, byte[] postbuf, byte[] mask, byte[] gate);



    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFSetDestIp(byte[] ip, byte[] port);
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFGetDestIp(byte[] ip, byte[] port);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFSetWorkMode(byte mode);
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFGetWorkMode(byte[] mode);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFSetBeep(byte mode);
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFGetBeep(byte[] mode);



    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int TCPConnect(StringBuilder ip, uint hostport);
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static void TCPDisconnect();

    //打开串口
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static int ComOpenWithBaud(int port, int baudrate);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int ComOpen(int comName);
    //关闭串口
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static void ClosePort();

    /**********************************************************************************************************
       * 功能：获取硬件版本号
       * 输出：version[0]--版本号长度 ,  version[1--x]--版本号
       *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetHardwareVersion(byte[] version);
    /**********************************************************************************************************
      * 功能：获取软件版本号
      * 输出：version[0]--版本号长度 ,  version[1--x]--版本号
      *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetSoftwareVersion(byte[] version);
    /**********************************************************************************************************
       * 功能：获取ID号
       * 输出：id--整型ID号
       *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetDeviceID(ref int id);

    /**********************************************************************************************************
    * 功能：设置功率
    * 输入：saveflag  -- 1:保存设置   0：不保存
    * 输入：uPower -- 功率（DB）
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetPower(byte save, byte uPower);
    /**********************************************************************************************************
    * 功能：设置天线功率
    * 输入：saveflag  -- 1:保存设置   0：不保存
    * 输入：num -- 天线编号(1~N)
            read_power -- 接收功率（DB）
            write_power -- 发送功率（DB）
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetAntennaPower(byte save, byte num, byte read_power, byte write_power);
    /**********************************************************************************************************
    * 功能：获取功率
    * 输出：uPower -- 功率（DB）
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetPower(ref byte uPower);
    /**********************************************************************************************************
    * 功能：获取天线功率
    * 输出：ppower -- 天线功率,格式为（天线编号+读功率+写功率+天线编号+读功率+写功率+.......+天线编号+读功率+写功率）
            nBytesReturned -- ppower数据长度 
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetAntennaPower(byte[] ppower, int[] nBytesReturned);


    /**********************************************************************************************************
    * 功能：设置跳频
    * 输入：nums -- 跳频个数
    * 输入：Freqbuf--频点数组（整型） ，如920125，921250.....
   *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetJumpFrequency(byte nums, int[] Freqbuf);
    /**********************************************************************************************************
    * 功能：获取跳频
    * 输出：Freqbuf[0]--频点个数， Freqbuf[1]..[x]--频点数组（整型）
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetJumpFrequency(int[] Freqbuf);
    /**********************************************************************************************************
    * 功能：设置Gen2参数
    * 输入：
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetGen2(byte Target, byte Action, byte T, byte Q, byte StartQ, byte MinQ, byte MaxQ, byte D, byte C, byte P, byte Sel, byte Session, byte G, byte LF);
    /**********************************************************************************************************
    * 功能：获取Gen2参数
    * 输入：
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetGen2(ref byte Target, ref byte Action, ref byte T, ref byte Q, ref byte StartQ, ref byte MinQ, ref byte MaxQ, ref byte D, ref byte Coding, ref byte P, ref byte Sel, ref byte Session, ref byte G, ref byte LF);
    /**********************************************************************************************************
    * 功能：设置CW
    * 输入：flag -- 1:开CW，  0：关CW
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetCW(byte flag);
    /**********************************************************************************************************
    * 功能：获取CW
    * 输出：flag -- 1:开CW，  0：关CW
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetCW(ref byte flag);

    /**********************************************************************************************************
    * 功能：天线设置
    * 输入：saveflag -- 1:掉电保存，  0：不保存
    * 输入：buf--2bytes, 共16bits, 每bit 置1选择对应天线
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetANT(byte saveflag, byte[] buf);

    /**********************************************************************************************************
    * 功能：获取天线设置
    * 输出：buf--2bytes, 共16bits,
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetANT(byte[] buf);

    /**********************************************************************************************************
    * 功能：区域设置
    * 输入：saveflag -- 1:掉电保存，  0：不保存
    * 输入：region -- 0x01(China1),0x02(China2),0x04(Europe),0x08(USA),0x16(Korea),0x32(Japan)
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetRegion(byte saveflag, byte region);

    /**********************************************************************************************************
    * 功能：获取区域设置
    * 输出：region -- 0x01(China1),0x02(China2),0x04(Europe),0x08(USA),0x16(Korea),0x32(Japan)
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetRegion(ref byte region);

    /**********************************************************************************************************
    * 功能：获取当前温度
    * 输出：temperature -- 整型
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetTemperature(ref int temperature);

    /**********************************************************************************************************
    * 功能：设置温度保护
    * 输入：flag -- 1:温度保护， 0：无温度保护
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetTemperatureProtect(byte flag);
    /**********************************************************************************************************
    * 功能：获取温度保护
    * 输出：flag -- 1:温度保护， 0：无温度保护
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetTemperatureProtect(ref byte flag);
    /**********************************************************************************************************
    * 功能：设置天线工作时间
    * 输入：antnum -- 天线号
    * 输入：saveflag -- 1:掉电保存， 0：不保存
    * 输入：WorkTime -- 工作时间 ，单位ms, 范围 10-65535ms
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetANTWorkTime(byte antnum, byte saveflag, int WorkTime);
    /**********************************************************************************************************
    * 功能：获取天线工作时间
    * 输入：antnum -- 天线号
    * 输出：WorkTime -- 工作时间 ，单位ms, 范围 10-65535ms
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetANTWorkTime(byte antnum, ref int WorkTime);
    /**********************************************************************************************************
    * 功能：设置链路组合
    * 输入：saveflag -- 1:掉电保存， 0：不保存
    * 输入：mode --  0:DSB_ASK/FM0/40KHZ , 1:PR_ASK/Miller4/250KHZ , 2:PR_ASK/Miller4/300KHZ, 3:DSB_ASK/FM0/400KHZ
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetRFLink(byte saveflag, byte mode);

    /**********************************************************************************************************
    * 功能：获取链路组合
    * 输出：mode --  0:DSB_ASK/FM0/40KHZ , 1:PR_ASK/Miller4/250KHZ , 2:PR_ASK/Miller4/300KHZ, 3:DSB_ASK/FM0/400KHZ
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetRFLink(ref byte uMode);
    /**********************************************************************************************************
    * 功能：设置FastID功能
    * 输入：flag -- 1:开启， 0：关闭
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetFastID(byte flag);
    /**********************************************************************************************************
    * 功能：获取FastID功能
    * 输出：flag -- 1:开启， 0：关闭
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetFastID(ref byte flag);
    /**********************************************************************************************************
    * 功能：设置Tagfocus功能
    * 输入：flag -- 1:开启， 0：关闭
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetTagfocus(byte flag);
    /**********************************************************************************************************
    * 功能：获取Tagfocus功能
    * 输出：flag -- 1:开启， 0：关闭
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetTagfocus(ref byte flag);
    /**********************************************************************************************************
    * 功能：设置软件复位
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetSoftReset();
    /**********************************************************************************************************
    * 功能：设置Dual和Singel模式
    * 输入：saveflag -- 1:掉电保存， 0：不保存
    * 输入：mode -- 1:设置Singel模式， 0：设置Dual模式
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetDualSingelMode(byte saveflag, byte mode);
    /**********************************************************************************************************
    * 功能：获取Dual和Singel模式
    * 输出：mode -- 1:设置Singel模式， 0：设置Dual模式
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetDualSingelMode(ref byte mode);
    /**********************************************************************************************************
    * 功能：设置寻标签过滤设置
    * 输入：saveflag -- 1:掉电保存， 0：不保存
    * 输入：bank --  0x01:EPC , 0x02:TID, 0x03:USR
    * 输入：startaddr 起始地址，单位：字节
    * 输入：datalen 数据长度， 单位:字节
    * 输入：databuf 数据
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetFilter(byte saveflag, byte bank, int startaddr, int datalen, byte[] databuf);
    /**********************************************************************************************************
    * 功能：设置EPC和TID模式
    * 输入：saveflag -- 1:掉电保存， 0：不保存
    * 输入：mode -- 1：开启EPC和TID， 0:关闭
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetEPCTIDMode(byte saveflag, byte mode);
    /**********************************************************************************************************
    * 功能：获取EPC和TID模式
    * 输出：mode -- 1：开启EPC和TID， 0:关闭
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetEPCTIDMode(ref byte mode);

    /**********************************************************************************************************
   * 功能：恢复出厂设置
   *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetDefaultMode();
    /**********************************************************************************************************
    * 功能：单次盘存标签
    * 输出：uLenUii -- UII长度
    * 输出：uUii -- UII数据
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFInventorySingle(ref byte uLenUii, byte[] uUii);
    /**********************************************************************************************************
    * 功能：连续盘存标签
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFInventory();
    /**********************************************************************************************************
    * 功能：停止盘存标签
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFStopGet();
    /**********************************************************************************************************
      * 功能：获取连续盘存标签数据
      * 输出：uLenUii -- UII长度
      * 输出：uUii -- UII数据
      *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHF_GetReceived_EX(ref int uLenUii, byte[] uUii);
    /**********************************************************************************************************
    * 功能：读标签数据区
    * 输入：uAccessPwd -- 4字节密码
    * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
    * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
    * 输入：FilterLen -- 启动过滤的长度， 单位：字节
    * 输入：FilterData -- 启动过滤的数据
    * 输入：uBank -- 读取数据的bank
    * 输入：uPtr --  读取数据的起始地址， 单位：字
    * 输入：uCnt --  读取数据的长度， 单位：字
    * 输出：uReadDatabuf --  读取到的数据
    * 输出：uReadDataLen --  读取到的数据长度
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFReadData(byte[] uAccessPwd,
         byte FilterBank,
         int FilterStartaddr,
         int FilterLen,
         byte[] FilterData,
         byte uBank,
         int uPtr,
         int uCnt,
         byte[] uReadDatabuf,
         ref int uReadDataLen);

    /**********************************************************************************************************
      * 功能：写标签数据区
      * 输入：uAccessPwd -- 4字节密码
      * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
      * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：bit
      * 输入：FilterLen -- 启动过滤的长度， 单位：bit
      * 输入：FilterData -- 启动过滤的数据
      * 输入：uBank -- 写入数据的bank
      * 输入：uPtr --  写入数据的起始地址， 单位：字
      * 输入：uCnt --  写入数据的长度， 单位：字
      * 输入：uWriteDatabuf --  写入的数据
      *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFWriteData(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte uBank, int uPtr, byte uCnt, byte[] uDatabuf);
    /**********************************************************************************************************
    * 功能：LOCK标签
    * 输入：uAccessPwd -- 4字节密码
    * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
    * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
    * 输入：FilterLen -- 启动过滤的长度， 单位：字节
    * 输入：FilterData -- 启动过滤的数据
    * 输入：lockbuf --  3字节，第0-9位为Action位， 第10-19位为Mask位
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFLockTag(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte[] lockbuf);

    /**********************************************************************************************************
    * 功能：KILL标签
    * 输入：uAccessPwd -- 4字节密码
    * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
    * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
    * 输入：FilterLen -- 启动过滤的长度， 单位：字节
    * 输入：FilterData -- 启动过滤的数据
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFKillTag(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData);

    /**********************************************************************************************************
      * 功能：BlockWrite 特定长度的数据到标签的特定地址
      * 输入：uAccessPwd -- 4字节密码
      * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
      * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
      * 输入：FilterLen -- 启动过滤的长度， 单位：字节
      * 输入：FilterData -- 启动过滤的数据
      * 输入：uBank -- 块号  1：EPC, 2:TID, 3:USR
      * 输入：uPtr --  写入数据的起始地址， 单位：字
      * 输入：uCnt --   写入数据的长度， 单位：字
      * 输入：uWriteDatabuf --  写入的数据
      *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFBlockWriteData(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte uBank, int uPtr, int uCnt, byte[] uDatabuf);

    /**********************************************************************************************************
    * 功能：BlockErase 特定长度的数据到标签的特定地址
    * 输入：uAccessPwd -- 4字节密码
    * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
    * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
    * 输入：FilterLen -- 启动过滤的长度， 单位：字节
    * 输入：FilterData -- 启动过滤的数据
    * 输入：uBank -- 块号  1：EPC, 2:TID, 3:USR
    * 输入：uPtr --  读取数据的起始地址， 单位：字
    * 输入：uCnt --  读取数据的长度， 单位：字
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFBlockEraseData(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte uBank, int uPtr, byte uCnt);
    /**********************************************************************************************************
    * 功能：设置QT命令参数
    * 输入：uAccessPwd -- 4字节密码
    * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
    * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
    * 输入：FilterLen -- 启动过滤的长度， 单位：字节
    * 输入：FilterData -- 启动过滤的数据
    * 输入：QTData --  bit0：（0：表示无近距离控制 ， 1：表示启用近距离控制）  bit1：(0:表示启用private Memory map, 1：启用public memory map)
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetQT(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte QTData);
    /**********************************************************************************************************
    * 功能：获取QT命令参数
    * 输入：uAccessPwd -- 4字节密码
    * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
    * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
    * 输入：FilterLen -- 启动过滤的长度， 单位：字节
    * 输入：FilterData -- 启动过滤的数据
    * 输出：QTData --  bit0：（0：表示无近距离控制 ， 1：表示启用近距离控制）  bit1：(0:表示启用private Memory map, 1：启用public memory map)
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetQT(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, ref byte QTData);
    /**********************************************************************************************************
    * 功能：QT标签读操作
    * 输入：uAccessPwd -- 4字节密码
    * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
    * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
    * 输入：FilterLen -- 启动过滤的长度， 单位：字节
    * 输入：FilterData -- 启动过滤的数据
    * 输入：QTData --  bit0：（0：表示无近距离控制 ， 1：表示启用近距离控制）  
    * 输入：uBank -- 块号  1：EPC, 2:TID, 3:USR
    * 输入：uPtr --  读取数据的起始地址， 单位：字
    * 输入：uCnt --  读取数据的长度， 单位：字
    * 输出：uReadDatabuf --  读出的数据
    * 输出：uReadDataLen --  读出的数据长度
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFReadQT(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte QTData, byte uBank, int uPtr, byte uCnt, byte[] uReadDatabuf, ref byte uReadDataLen);
    /**********************************************************************************************************
    * 功能：QT标签写操作
    * 输入：uAccessPwd -- 4字节密码
    * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
    * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
    * 输入：FilterLen -- 启动过滤的长度， 单位：字节
    * 输入：FilterData -- 启动过滤的数据
    * 输入：QTData --  bit0：（0：表示无近距离控制 ， 1：表示启用近距离控制）  
    * 输入：uBank -- 块号  1：EPC, 2:TID, 3:USR
    * 输入：uPtr --  读取数据的起始地址， 单位：字
    * 输入：uCnt --  读取数据的长度， 单位：字
    * 输入：uWriteDatabuf --  写入的数据
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFWriteQT(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte QTData, byte uBank, int uPtr, byte uCnt, byte[] uDatabuf);
    /**********************************************************************************************************
    * 功能：Block Permalock操作
    * 输入：uAccessPwd -- 4字节密码
    * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
    * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
    * 输入：FilterLen -- 启动过滤的长度， 单位：字节
    * 输入：FilterData -- 启动过滤的数据
    * 输入：ReadLock --  bit0：（0：表示Read ， 1：表示Permalock）  
    * 输入：uBank -- 块号  1：EPC, 2:TID, 3:USR
    * 输入：uPtr --  Block起始地址 ，单位为16个块
    * 输入：uRange --  Block范围，单位为16个块
    * 输入：uMaskbuf -- 块掩码数据，2个字节，16bit 对应16个块
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFBlockPermalock(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte ReadLock, byte uBank, int uPtr, byte uRange, byte[] uMaskbuf);

    /**********************************************************************************************************
    * 功能：激活或失效EM4124标签
    * 输入：cmd --整形
    * 输入：uAccessPwd -- 4字节密码
    * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
    * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
    * 输入：FilterData -- 启动过滤的数据
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFDeactivate(int cmd, byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData);

    /**********************************************************************************************************
    * 功能：获取协议类型  
    * 输出：type  0x00 表示 ISO18000-6C 协议,    0x01 表示 GB/T 29768 国标协议,      0x02 表示 GJB 7377.1 国军标协议

    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetProtocolType(byte[] type);


    /**********************************************************************************************************
    * 功能：设置协议类型
    * 输入：type  0x00 表示 ISO18000-6C 协议,0x01 表示 GB/T 29768 国标协议,0x02 表示 GJB 7377.1 国军标协议
    * 
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetProtocolType(byte type);
    /**********************************************************************************************************
    * 功能：国标LOCK标签
    * 输入：uAccessPwd -- 4字节密码
    * 输入：FilterBank -- 启动过滤的bank号， 1：EPC, 2:TID, 3:USR
    * 输入：FilterStartaddr -- 启动过滤的起始地址， 单位：字节
    * 输入：FilterLen -- 启动过滤的长度， 单位：字节
    * 输入：FilterData -- 启动过滤的数据

    * 输入：memory 存储区：  0x00 表示标签信息区,   0x10 表示编码区,   0x20 表示安全区,   0x3x 表示用户区 0x30-0x3F 表示用户区编号 0 到编号 15
            config 配置：    0x00 表示配置存储区属性,    0x01 表示配置安全模式


            action:  

                  配置存储区属性:  0x00:可读可写,  0x01:可读不可写,  0x02:不可读可写,  0x03:不可读不可写

                  配置安全模式:    0x00:保留,   0x01:不需要鉴别,   0x02:需要鉴别,不需要安全通信,   0x03:需要鉴别,需要安全通信

    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGBTagLock(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte memory, byte config, byte action);



    /**********************************************************************************************************
     * 功能：获取继电器和 IO 控制输出设置状态
     * 输入：outData[0]:    0:低电平   1：高电平
             outData[1]:    0:低电平   1：高电平
       返回值：2：数据长度    -1：获取失败
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetIOControl(byte[] inputData);

    /**********************************************************************************************************
    * 功能：继电器和 IO 控制输出设置
    * 输入：output1:    0:低电平   1：高电平
            output2:    0:低电平   1：高电平
            outStatus： 0：断开    1：闭合
      返回值：0：设置成功     -1：设置失败
    * 
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetIOControl(byte output1, byte output2, byte outStatus);



    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetOutputIO(byte[] output, byte outputLen);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetIOStatus(byte[] statusData, int[] dataLen);



    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFBuildDateTime(byte[] build_date, byte[] build_time);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetVersionCode(byte[] datetime);


    /**********************************************************************************************************
    * 功能：设置连续寻卡工作及等待时间
    * 输入：DByte4 为掉电保存标志，0 表示不保存，1 表示保存；DByte3、DByte2 为工作时间，高字节在前，DByte1、DByte0 为等待时间，高字节在前


      返回值：0：设置成功     -1：设置失败

    * 
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetWorkTime(byte save, byte work1, byte work2, byte wait1, byte wait2);

    /**********************************************************************************************************
    * 功能：获取连续寻卡工作及等待时间
    * 输出：DByte[0]、DByte[1] 表示工作时间；DByte[2]、DByte[3] 表示等待时间，单位为 ms，高位在前，最大 65535ms

      返回值：4：数据长度    -1：获取失败
    * 
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetWorkTime(byte[] data);



    /**********************************************************************************************************
    * 功能：设置EPC TID USER模式

    * 输入：saveflag -- 1:掉电保存， 0：不保存

    * 输入：Memory 0x00，表示关闭； 0x01，表示开启EPC+TID模式（默认 地址为 0x00, 长 度 为 6 个 字 ） ； 0x02, 表 示 开 启EPC+TID+USER模式

    * 输入：address 为USER区的起始地址（单位为字）
    * 输入：为USER区的长度（单位为字）
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetEPCTIDUSERMode(byte saveflag, byte memory, byte address, byte lenth);
    /**********************************************************************************************************
    * 功能：获取EPC TID USER 模式
    * 输入：rev1 :保留数据，传入0
    * 输入：rev2 :保留数据，传入0
    * 输出：mode[0]，Memory 0x00，表示关闭； 0x01，表示开启EPC+TID模式（默认 地址为 0x00, 长 度 为 6 个 字 ） ； 0x02, 表 示 开 启EPC+TID+USER模式
    * 输出：mode[1]address 为USER区的起始地址（单位为字）
    * 输出：mode[2]为USER区的长度（单位为字）
    * 返回值：3：正确，其它：错误
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetEPCTIDUSERMode(byte rev1, byte rev2, byte[] mode);





    /*
    初始化温度标签
    return: 0--success, -1--unknow error, others--error code
    mask_bank：掩码的数据区(0x00 为 Reserve 0x01 为 EPC， 0x02 表示 TID， 0x03 表示USR)。
    mask_addr：为掩码的地址(bit为单位)，高字节在前。
    mask_len：为掩码的长度(bit为单位)，高字节在前。
    mask_data：为掩码数据，mask_len为0时，这里没有数据
    */

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFInitRegFile(byte mask_bank, int mask_addr, int mask_len, byte[] mask_data);

    /*
    读取标签温度
    return: 0--success, -1--unknow error, others--error code
    mask_bank：掩码的数据区(0x00 为 Reserve 0x01 为 EPC， 0x02 表示 TID， 0x03 表示USR)。
    mask_addr：为掩码的地址(bit为单位)，高字节在前。
    mask_len：为掩码的长度(bit为单位)，高字节在前。
    mask_data：为掩码数据，mask_len为0时，这里没有数据
    temp:output,the point of temperature
    */
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFReadTagTemp(byte mask_bank, int mask_addr, int mask_len, byte[] mask_data, float[] outtemp);

    //level:0-close log output, 3-base log,4-detail log
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int SetLogLevel(int level);


    /**********************************************************************************************************
    * 功能：设置是否保存传输过程中的日志文件，默认不保存
    * 输入： 
    *save -- 0-不保存、1-保存日志文件
    *返回值：无 
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int SaveLogFile(int lsaveevel);



    // zjx 20191127 UHF升级--- start ---
    /*
        flag: 0,upgrade reader application
              1,upgrade UHF module
              2,upgrade reader bootloader 
              3,upgrade Ex10 SDK firmware
        */
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFJump2Boot(byte flag);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFStartUpd();

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFUpdating(byte[] buf);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHF_Upgrade(byte[] buf, int length);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFStopUpdate();



    /**********************************************************************************************************
* 功能：获取读写器软件版本号
* 输出：version[0]--版本号长度 ,  version[1--x]--版本号
*********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetReaderVersion(byte[] version);


    // zjx 20191127 UHF升级--- end ---


    /****************************  zjx 20200416 触发工作模式参数设置获取  -------- start -------- **************************/
    /**********************************************************************************************************
    * 功能：设置触发工作模式参数
    * 输入：
            para[0],	     触发IO：0x00表示触发输入1；0x01表示触发输入2。
            para[1],para[2]  触发工作时间：表示有触发输入时读卡工作时间，单位是10ms，高字节先，低字节后。
            para[3],para[4]	触发时间间隔：表示触发工作时间结束后，间隔多久再判断触发输入IO口进行触发。
            para[5]     	标签输出方式：0x00表示串口输出，0x01表示UDP输出
    * 
    * 返回值：   0:成功    其它：失败
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFSetWorkModePara(byte[] para);


    /**********************************************************************************************************
    * 功能：获取触发工作模式参数
    * 输出：
            para[0],	     触发IO：0x00表示触发输入1；0x01表示触发输入2。
            para[1],para[2]  触发工作时间：表示有触发输入时读卡工作时间，单位是10ms，高字节先，低字节后。
            para[3],para[4]	 触发时间间隔：表示触发工作时间结束后，间隔多久再判断触发输入IO口进行触发。
            para[5]     	 标签输出方式：0x00表示串口输出，0x01表示UDP输出
    * 
    * 返回值：   0:成功    其它：失败
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int UHFGetWorkModePara(byte[] para);


    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UsbOpen();
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static void UsbClose();

    /****************************  zjx 20200416 触发工作模式参数设置获取   -------- end -------- **************************/




    /***************************************************************************************/
    //获取当前连接的ip信息
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static int LinkGetInfo(byte[] info, int len);

    //选择要操作的id，根据LinkGetInfo获取id信息
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static int LinkSelect(int id);

    //获取当前可以操作的id
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static int LinkGetSelected();

    //断开所以连接
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static void LinkCloseAll();


    /**********************************************************************************************************
    * function:get status of antennas linked
    * out:link_status,status of antenna linked,bit0~bit15 indicate antenna1~antenna16,bit 0/not link 1/linked
    * return：0：success    -1：failure
    *********************************************************************************************************/
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static int UHFGetAntennaLinkStatus(short[] link_status);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static int UHFVerifyVoltage(int[] value);




    /*
       按块写无源电子标签带水墨屏显示
       pwd：4 个字节的块写密码
       sector：掩码的数据区(0x00 为 Reserve，0x01 为 EPC，0x02 表示 TID，0x03 表示 USR)。
       mask_addr：为掩码的地址。
       mask_len：为掩码的长度。
       mask_data：为掩码数据。
       w_addr：为写入数据区的地址（单位是字）。
       w_len：为写入的数据长度（单位是字）。
       w_data：为写入的具体数据（txt 文件中的数据）。
       */
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static int UHFWriteScreenBlockData(byte[] pwd, byte sector, short mask_addr, short mask_len, byte[] mask_data, byte type,
        short w_addr, short w_len, byte[] w_data);



    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static int UHFUploadUserParam(byte[] param, short paramLen);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static int UHFDownloadUserParam(byte[] param, short[] paramLen);


    //return 0,no data, > 0 tag length, < 0 error code
    //tdata tag data, type+length+content+...+type+length+content
    //type:1-epc,2-tid,3-user,4-rssi,5-antenna,6-id
    //
    // #define CONTENT_TYPE_INVALID        0
    // #define CONTENT_TYPE_EPC            1
    // #define CONTENT_TYPE_TID            2
    // #define CONTENT_TYPE_USER           3
    // #define CONTENT_TYPE_RSSI           4
    // #define CONTENT_TYPE_ANT            5
    // #define CONTENT_TYPE_ID             6
    // #define CONTENT_TYPE_IP             7
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFGetTagData(byte[] tdata, int recvlen);


    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFInventorySingle(int id);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFStopSingle(int id);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFInventoryById(int id);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    private extern static int UHFStopById(int id);



    //   typedef enum{CELL_INVALID=0, CELL_CONNECT_ID=1, CELL_CONNECT_IP, CELL_UHF_PC, CELL_UHF_RSSI, CELL_UHF_ANTENNA, CELL_UHF_EPC, CELL_UHF_TID, CELL_UHF_USER,CELL_UHF_RESERVE,CELL_BARCODE, CELL_UHF_SENSOR} CELL_DATA_TYPE;









    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void OnDataReceived(IntPtr epc, short recvLen);//[MarshalAs(UnmanagedType.LPArray, SizeConst = 4096)]


    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static void setOnDataReceived(OnDataReceived onDataRecved);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.Cdecl)]
    public extern static int PrintTextToCursor(int codeType, byte[] text, short len);

    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.StdCall)]
    public extern static int BindUDP(int bindport);
    [DllImport("UHFAPI.dll", CallingConvention = CallingConvention.StdCall)]
    public extern static void UnbindUDP();

    //typedef enum {CHAR_CODE_ASCII=1, CHAR_CODE_GB2312=2, CHAR_CODE_GBK=3, CHAR_CODE_BIG5=4, CHAR_CODE_UTF8=5}ENUM_CHAR_CODE_TYPE;


    //extern "C" UHFAPI_API int PrintTextToCursor(ENUM_CHAR_CODE_TYPE type, const char *text, unsigned short len);

    private static void recvDataCallback(IntPtr epc, short recvLen)
    {
        Console.WriteLine("entry call back");
    }











    private static UHFAPI uhf = null;
    internal UHFAPI() { }
    public static UHFAPI getInstance()
    {
        if (uhf == null)
            uhf = new UHFAPI();
        return uhf;
    }

    #region usb
    public bool OpenUsb()
    {
        int result = UsbOpen();
        if (result == 0)
        {
            return true;
        }
        return false;
    }
    public void CloseUsb()
    {
        UsbClose();
    }
    #endregion


    #region 协议
    public bool SetProtocol(byte type)
    {
        if (UHFSetProtocolType(type) == 0)
        {
            return true;
        }
        return false;
    }
    public int GetProtocol()
    {
        byte[] type = new byte[1];
        if (UHFGetProtocolType(type) == 0)
        {
            return type[0];
        }
        return -1;
    }
    #endregion


    #region  国标标签Lock



    /// 
    /// <summary>
    /// 国标标签Lock
    /// </summary>
    /// <param name="uAccessPwd">4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：bit</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：字节</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <param name="memory"></param>
    /// <param name="config"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool GBTagLock(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte memory, byte config, byte action)
    {
        if (UHFGBTagLock(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData, memory, config, action) == 0)
        {
            return true;
        }
        return false;
    }

    #endregion



    #region TCPIP
    /// <summary>
    /// 连接主机
    /// </summary>
    /// <param name="ip">ip地址</param>
    /// <param name="port">端口</param>
    /// <returns>返回true表示成功，返回false表示失败</returns>
    public bool TcpConnect(string ip, uint port)
    {

        if (ip == null || ip == "")
        {
            return false;
        }
        ip = ip.Trim();

        if (!StringUtils.isIP(ip))
        {
            return false;
        }
        StringBuilder bIp = new StringBuilder();
        bIp.Append(ip);

        Console.WriteLine("TCPConnect(" + ip + ", " + port + ")");
        int result = TCPConnect(bIp, port);
        if (result == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 断开主机
    /// </summary>
    /// <returns></returns>
    public void TcpDisconnect()
    {
        TCPDisconnect();
    }


    //设置蜂鸣器 工作模式：0x00表示关闭蜂鸣器；0x01表示打开蜂鸣器
    public bool UHFSetBuzzer(byte mode)
    {
        if (UHFSetBeep(mode) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    //获取蜂鸣器 工作模式：0x00表示关闭蜂鸣器；0x01表示打开蜂鸣器
    public bool UHFGetBuzzer(byte[] mode)
    {
        if (UHFGetBeep(mode) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public bool SetLocalIP(string ip, int port, string mask, string gate)
    {

        if (!StringUtils.isIP(ip))
        {
            return false;
        }
        if (!StringUtils.isIP(mask))
        {
            return false;
        }
        if (!StringUtils.isIP(gate))
        {
            return false;
        }
        byte[] bPort = new byte[2];
        byte[] bIP = new byte[4];
        byte[] bmask = new byte[4];
        byte[] bgate = new byte[4];

        string hexPort = DataConvert.DecimalToHexString(port);
        bPort = DataConvert.HexStringToByteArray("0000".Substring(0, 4 - hexPort.Length) + hexPort);

        string[] strIp = ip.Split('.');
        for (int k = 0; k < strIp.Length; k++)
        {
            bIP[k] = byte.Parse(strIp[k]);
        }

        string[] temp = mask.Split('.');
        for (int k = 0; k < temp.Length; k++)
        {
            bmask[k] = byte.Parse(temp[k]);
        }

        temp = gate.Split('.');
        for (int k = 0; k < temp.Length; k++)
        {
            bgate[k] = byte.Parse(temp[k]);
        }



        if (UHFSetIp(bIP, bPort, bmask, bgate) == 0)//, bmask, bgate
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GetLocalIP(StringBuilder ip, StringBuilder port, StringBuilder mask, StringBuilder gate)
    {
        byte[] sIP = new byte[4];
        byte[] sPort = new byte[2];

        byte[] sMask = new byte[4];
        byte[] sGate = new byte[4];
        int startTime = Environment.TickCount;
        if (UHFGetIp(sIP, sPort, sMask, sGate) == 0)
        {
            // MessageBoxEx.Show((Environment.TickCount-startTime)+"");


            if (ip != null)
            {
                ip.Append(sIP[0]);
                ip.Append(".");
                ip.Append(sIP[1]);
                ip.Append(".");
                ip.Append(sIP[2]);
                ip.Append(".");
                ip.Append(sIP[3]);
            }
            if (port != null)
            {
                string hexPort = DataConvert.ByteArrayToHexString(sPort).Replace(" ", "");
                int iPort = Convert.ToInt32(hexPort, 16);
                port.Append(iPort);
            }

            if (sMask != null)
            {
                if (sMask[0] == 0 && sMask[1] == 0 && sMask[2] == 0 && sMask[3] == 0)
                {
                    sMask[0] = 255;
                    sMask[1] = 255;
                    sMask[2] = 255;
                    sMask[3] = 0;
                }
                mask.Append(sMask[0]);
                mask.Append(".");
                mask.Append(sMask[1]);
                mask.Append(".");
                mask.Append(sMask[2]);
                mask.Append(".");
                mask.Append(sMask[3]);
            }


            if (sGate != null)
            {
                if (sGate[0] == 0 && sGate[1] == 0 && sGate[2] == 0 && sGate[3] == 0)
                {
                    sGate[0] = sIP[0];
                    sGate[1] = sIP[1];
                    sGate[2] = sIP[2];
                    sGate[3] = 1;
                }
                gate.Append(sGate[0]);
                gate.Append(".");
                gate.Append(sGate[1]);
                gate.Append(".");
                gate.Append(sGate[2]);
                gate.Append(".");
                gate.Append(sGate[3]);
            }


            return true;
        }
        else
        {
            return false;
        }
    }



    public bool SetDestIP(string ip, int port)
    {
        if (ip == null || ip == "")
        {
            return false;
        }
        ip = ip.Trim();

        if (!StringUtils.isIP(ip))
        {
            return false;
        }
        byte[] bPort = new byte[2];
        byte[] bIP = new byte[4];

        string hexPort = DataConvert.DecimalToHexString(port);
        bPort = DataConvert.HexStringToByteArray("0000".Substring(0, 4 - hexPort.Length) + hexPort);

        string[] strIp = ip.Split('.');
        for (int k = 0; k < strIp.Length; k++)
        {
            bIP[k] = byte.Parse(strIp[k]);
        }


        if (UHFSetDestIp(bIP, bPort) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GetDestIP(StringBuilder ip, StringBuilder port)
    {
        byte[] sIP = new byte[4];
        byte[] sPort = new byte[2];

        if (UHFGetDestIp(sIP, sPort) == 0)
        {
            if (ip != null)
            {
                ip.Append(sIP[0]);
                ip.Append(".");
                ip.Append(sIP[1]);
                ip.Append(".");
                ip.Append(sIP[2]);
                ip.Append(".");
                ip.Append(sIP[3]);
            }
            if (port != null)
            {
                string hexPort = DataConvert.ByteArrayToHexString(sPort).Replace(" ", "");
                int iPort = Convert.ToInt32(hexPort, 16);
                port.Append(iPort);
            }
            return true;
        }
        else
        {
            return false;
        }
    }


    #endregion

    #region 串口、版本号、ID
    /// <summary>
    /// 打开串口
    /// </summary>
    /// <param name="com">串口号：0,1,2....</param>
    /// <returns>返回0表示成功，返回1表示失败</returns>
    public bool Open(int comName)
    {
        int result = ComOpen(comName);
        if (result == 0)
        {
            return true;
        }
        return false;
    }

    public bool Open()
    {
        return false;
    }
    /// <summary>
    /// 关闭串口
    /// </summary>
    /// <returns></returns>
    public bool Close()
    {
        ClosePort();
        return true;
    }




    /// <summary>
    /// 获取硬件版本
    /// </summary>
    /// <returns></returns>
    public string GetHardwareVersion()
    {
        byte[] version = new byte[50];
        if (UHFGetHardwareVersion(version) == 0)
        {
            int len = version[0];
            byte[] versionTemp = new byte[len];
            Array.Copy(version, 1, versionTemp, 0, len);
            return System.Text.Encoding.ASCII.GetString(versionTemp);// DataConvert.ByteArrayToHexString(versionTemp);
        }
        return string.Empty;
    }
    /// <summary>
    /// 获取软件版本
    /// </summary>
    /// <returns></returns>
    public string GetSoftwareVersion()
    {
        byte[] version = new byte[50];
        if (UHFGetSoftwareVersion(version) == 0)
        {
            int len = version[0];
            byte[] versionTemp = new byte[len];
            Array.Copy(version, 1, versionTemp, 0, len);
            return System.Text.Encoding.ASCII.GetString(versionTemp);//DataConvert.ByteArrayToHexString(versionTemp);
        }
        return string.Empty;
    }
    public string GetSTM32Version()
    {
        byte[] version = new byte[50];
        if (UHFGetReaderVersion(version) == 0)
        {
            int len = version[0];
            byte[] versionTemp = new byte[len];
            Array.Copy(version, 1, versionTemp, 0, len);
            return System.Text.Encoding.ASCII.GetString(versionTemp);//DataConvert.ByteArrayToHexString(versionTemp);
        }
        return string.Empty;
    }


    /// <summary>
    /// 获取设备ID
    /// </summary>
    /// <returns>id--整型ID号</returns>
    public int GetUHFGetDeviceID()
    {
        int id = -1;
        UHFGetDeviceID(ref id);
        return id;
    }
    #endregion

    #region 频率、功率
    /// <summary>
    /// 设置功率
    /// </summary>
    /// <param name="save">1:保存设置   0：不保存</param>
    /// <param name="uPower">功率（DB）</param>
    /// <returns></returns>
    public bool SetPower(byte save, byte uPower)
    {
        if (UHFSetPower(save, uPower) == 0)
            return true;
        return false;
    }
    /// <summary>
    /// 获取功率
    /// </summary>
    /// <param name="uPower">功率（DB）</param>
    /// <returns></returns>
    public bool GetPower(ref byte uPower)
    {
        if (UHFGetPower(ref uPower) == 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 设置功率
    /// </summary>
    /// <param name="save">1:保存设置   0：不保存</param>
    /// <param name="num">天线</param>
    /// <param name="uPower">功率（DB）</param>
    /// <returns></returns>
    public bool SetAntennaPower(byte save, byte num, byte uPower)
    {
        if (UHFSetAntennaPower(save, num, uPower, uPower) == 0)
            return true;
        return false;
    }
    /// <summary>
    /// 获取功率
    /// </summary>
    /// <param name="uPower">功率（DB）</param>
    /// <returns></returns>
    public bool GetAntennaPower(byte[] uPower)
    {
        byte[] data = new byte[100];
        int[] resultLen = new int[1];
        if (UHFGetAntennaPower(data, resultLen) == 0)
        {
            for (int k = 0; k < resultLen[0] / 3; k++)
            {
                uPower[k] = data[k * 3 + 1];
            }


            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取跳频
    /// </summary>
    /// <param name="Freqbuf">Freqbuf[0]--频点个数， Freqbuf[1]..[x]--频点数组（整型）</param>
    /// <returns></returns>
    public bool GetJumpFrequency(ref int[] Freqbuf)
    {
        int[] temp = new int[512];
        if (UHFGetJumpFrequency(temp) == 0)
        {
            int len = temp[0];
            int[] freqData = new int[len];
            Array.Copy(temp, 1, freqData, 0, len);
            Freqbuf = freqData;
            return true;
        }
        return false;
    }
    /// <summary>
    /// 设置跳频
    /// </summary>
    /// <param name="nums">跳频个数</param>
    /// <param name="Freqbuf">Freqbuf--频点数组（整型） ，如920125，921250.....</param>
    /// <returns></returns>
    public bool SetJumpFrequency(byte nums, int[] Freqbuf)
    {
        if (UHFSetJumpFrequency(nums, Freqbuf) == 0)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region session
    /// <summary>
    /// 设置session
    /// </summary>
    /// <param name="Target"></param>
    /// <param name="Action"></param>
    /// <param name="T"></param>
    /// <param name="Q"></param>
    /// <param name="StartQ"></param>
    /// <param name="MinQ"></param>
    /// <param name="MaxQ"></param>
    /// <param name="D"></param>
    /// <param name="C"></param>
    /// <param name="P"></param>
    /// <param name="Sel"></param>
    /// <param name="Session"></param>
    /// <param name="G"></param>
    /// <param name="LF"></param>
    /// <returns></returns>
    public bool SetGen2(byte Target, byte Action, byte T, byte Q,
                          byte StartQ, byte MinQ,
                          byte MaxQ, byte D, byte C, byte P,
                          byte Sel, byte Session, byte G, byte LF)
    {
        if (UHFSetGen2(Target, Action, T, Q, StartQ, MinQ, MaxQ, D, C, P, Sel, Session, G, LF) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取session
    /// </summary>
    /// <param name="Target"></param>
    /// <param name="Action"></param>
    /// <param name="T"></param>
    /// <param name="Q"></param>
    /// <param name="StartQ"></param>
    /// <param name="MinQ"></param>
    /// <param name="MaxQ"></param>
    /// <param name="D"></param>
    /// <param name="Coding"></param>
    /// <param name="P"></param>
    /// <param name="Sel"></param>
    /// <param name="Session"></param>
    /// <param name="G"></param>
    /// <param name="LF"></param>
    /// <returns></returns>
    public bool GetGen2(ref byte Target, ref byte Action, ref byte T, ref byte Q,
               ref byte StartQ, ref byte MinQ,
               ref byte MaxQ, ref byte D, ref byte Coding, ref byte P,
               ref byte Sel, ref byte Session, ref byte G, ref byte LF)
    {
        if (UHFGetGen2(ref Target, ref Action, ref T, ref Q, ref StartQ, ref MinQ, ref MaxQ, ref D, ref Coding, ref P, ref Sel, ref Session, ref G, ref LF) == 0)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region 连续波、天线、区域、模块温度、温度保护
    /// <summary>
    /// 设置CW
    /// </summary>
    /// <param name="flag">flag -- 1:开CW，  0：关CW</param>
    /// <returns></returns>
    public bool SetCW(byte flag)
    {
        if (UHFSetCW(flag) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 功能：获取CW
    /// </summary>
    /// <param name="flag">flag -- 1:开CW，  0：关CW</param>
    /// <returns></returns>
    public bool GetCW(ref byte flag)
    {
        if (UHFGetCW(ref flag) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 天线设置
    /// </summary>
    /// <param name="saveflag">saveflag -- 1:掉电保存，  0：不保存</param>
    /// <param name="buf">buf--2bytes, 共16bits, 每bit 置1选择对应天线</param>
    /// <returns></returns>
    public bool SetANT(byte saveflag, byte[] buf)
    {
        if (UHFSetANT(saveflag, buf) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取天线设置
    /// </summary>
    /// <param name="buf">buf--2bytes, 共16bits</param>
    /// <returns></returns>
    public bool GetANT(byte[] buf)
    {
        if (UHFGetANT(buf) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取天线设置
    /// </summary>
    /// <param name="buf">buf--2bytes, 共16bits</param>
    /// <returns></returns>
    public bool GetANTLinkStatus(short[] buf)
    {
        if (UHFGetAntennaLinkStatus(buf) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 区域设置
    /// </summary>
    /// <param name="saveflag">1:掉电保存，  0：不保存</param>
    /// <param name="region">0x01(China1),0x02(China2),0x04(Europe),0x08(USA),0x16(Korea),0x32(Japan)</param>
    /// <returns></returns>
    public bool SetRegion(byte saveflag, byte region)
    {
        if (UHFSetRegion(saveflag, region) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取区域设置
    /// </summary>
    /// <param name="region"> 0x01(China1),0x02(China2),0x04(Europe),0x08(USA),0x16(Korea),0x32(Japan)</param>
    /// <returns></returns>
    public bool GetRegion(ref byte region)
    {
        if (UHFGetRegion(ref region) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取模块温度
    /// </summary>
    /// <param name="temperature">回传的温度</param>
    /// <returns>返回true表示获取成功，temperature参数可以使用。返回false表示获取失败，temperature参数不可以使用</returns>
    public string GetTemperature()
    {
        int temperature = 0;
        if (UHFGetTemperature(ref temperature) == 0)
        {
            return temperature.ToString();
        }
        return string.Empty;
    }
    /// <summary>
    /// 设置温度保护
    /// </summary>
    /// <param name="flag">1:温度保护， 0：无温度保护</param>
    /// <returns></returns>
    public bool SetTemperatureProtect(byte flag)
    {
        if (UHFSetTemperatureProtect(flag) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取温度保护
    /// </summary>
    /// <param name="flag">1:温度保护， 0：无温度保护</param>
    /// <returns></returns>
    public bool GetTemperatureProtect(ref byte flag)
    {
        if (UHFGetTemperatureProtect(ref flag) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 设置天线工作时间
    /// </summary>
    /// <param name="antnum">天线号</param>
    /// <param name="saveflag">1:掉电保存， 0：不保存</param>
    /// <param name="WorkTime">工作时间 ，单位ms, 范围 10-65535ms</param>
    /// <returns></returns>
    public bool SetANTWorkTime(byte antnum, byte saveflag, int WorkTime)
    {
        if (UHFSetANTWorkTime(antnum, saveflag, WorkTime) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取天线工作时间
    /// </summary>
    /// <param name="antnum">天线号</param>
    /// <param name="WorkTime">工作时间 ，单位ms, 范围 10-65535ms</param>
    /// <returns></returns>
    public bool GetANTWorkTime(byte antnum, ref int WorkTime)
    {
        if (UHFGetANTWorkTime(antnum, ref WorkTime) == 0)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region 链路组合、FastID、Tagfocus、Dual和Singel模式、软件复位、恢复出厂设置

    /// <summary>
    /// 设置链路组合
    /// </summary>
    /// <param name="saveflag">1:掉电保存， 0：不保存</param>
    /// <param name="mode">0:DSB_ASK/FM0/40KHZ , 1:PR_ASK/Miller4/250KHZ , 2:PR_ASK/Miller4/300KHZ, 3:DSB_ASK/FM0/400KHZ</param>
    /// <returns></returns>
    public bool SetRFLink(byte saveflag, byte mode)
    {

        if (UHFSetRFLink(saveflag, mode) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取链路组合
    /// </summary>
    /// <param name="uMode">0:DSB_ASK/FM0/40KHZ , 1:PR_ASK/Miller4/250KHZ , 2:PR_ASK/Miller4/300KHZ, 3:DSB_ASK/FM0/400KHZ</param>
    /// <returns></returns>
    public bool GetRFLink(ref byte uMode)
    {
        if (UHFGetRFLink(ref uMode) == 0)
            return true;

        return false;
    }
    /// <summary>
    /// 设置FastID功能
    /// </summary>
    /// <param name="flag">1:开启， 0：关闭</param>
    /// <returns></returns>
    public bool SetFastID(byte flag)
    {
        if (UHFSetFastID(flag) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取FastID功能
    /// </summary>
    /// <param name="flag">1:开启， 0：关闭</param>
    /// <returns></returns>
    public bool GetFastID(ref byte flag)
    {
        if (UHFGetFastID(ref flag) == 0)
            return true;
        return false;

    }
    /// <summary>
    /// 设置Tagfocus功能
    /// </summary>
    /// <param name="flag">1:开启， 0：关闭</param>
    /// <returns></returns>
    public bool SetTagfocus(byte flag)
    {
        if (UHFSetTagfocus(flag) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取Tagfocus功能
    /// </summary>
    /// <param name="flag">1:开启， 0：关闭</param>
    /// <returns></returns>
    public bool GetTagfocus(ref byte flag)
    {
        if (UHFGetTagfocus(ref flag) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 设置软件复位
    /// </summary>
    /// <returns></returns>
    public bool SetSoftReset()
    {
        if (UHFSetSoftReset() == 0)
            return true;
        return false;
    }
    /// <summary>
    /// 设置Dual和Singel模式
    /// </summary>
    /// <param name="saveflag">1:掉电保存， 0：不保存</param>
    /// <param name="mode">1:设置Singel模式， 0：设置Dual模式</param>
    /// <returns></returns>
    public bool SetDualSingelMode(byte saveflag, byte mode)
    {
        if (UHFSetDualSingelMode(saveflag, mode) == 0)
            return true;
        return false;
    }
    /// <summary>
    /// 获取Dual和Singel模式
    /// </summary>
    /// <param name="mode">1:设置Singel模式， 0：设置Dual模式</param>
    /// <returns></returns>
    public bool GetDualSingelMode(ref byte mode)
    {
        if (UHFGetDualSingelMode(ref mode) == 0)
            return true;
        return false;
    }


    /// <summary>
    /// 恢复出厂设置
    /// </summary>
    /// <returns></returns>
    public bool SetDefaultMode()
    {
        if (UHFSetDefaultMode() == 0)
            return true;
        return false;
    }
    #endregion

    #region 读、写、锁、销毁
    /// <summary>
    /// 读取数据
    /// </summary>
    /// <param name="uAccessPwd">4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：字节</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：字节</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <param name="uBank">读取数据的bank</param>
    /// <param name="uPtr">读取数据的起始地址， 单位：字</param>
    /// <param name="uCnt">读取数据的长度， 单位：字</param>
    /// <returns>返回十六进制数据，读取失败返回""</returns>
    public string ReadData(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte uBank, int uPtr, int uCnt)
    {
        try
        {
            byte[] uReadDatabuf = new byte[2048]; ;
            int uReadDataLen = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("\r\n密码：" + DataConvert.ByteArrayToHexString(uAccessPwd));
            sb.Append("\r\n过滤数据块（ 1：EPC, 2:TID, 3:USR）：" + FilterBank);
            sb.Append("\r\n过滤起始地址：" + FilterStartaddr);
            sb.Append("\r\n过滤长度：" + FilterLen);
            sb.Append("\r\n过滤数据：" + DataConvert.ByteArrayToHexString(FilterData));
            sb.Append("\r\n");
            sb.Append("\r\n读取的数据块：" + uBank);
            sb.Append("\r\n读取的数据起始地址：" + uPtr);
            sb.Append("\r\n读取的数据长度：" + uCnt);
            sb.Append("\r\n");

            FileManage.WriterFile("C:\\Users\\Administrator\\Desktop\\UHFLog.txt", sb.ToString(), true);

            int result = UHFReadData(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData, uBank, uPtr, uCnt, uReadDatabuf, ref uReadDataLen);
            if (result == 0)
            {
                return DataConvert.ByteArrayToHexString(uReadDatabuf, uReadDataLen);
            }
        }
        catch (Exception ex)
        {

        }

        return string.Empty;
    }


    /// <summary>
    /// 写标签数据区
    /// </summary>
    /// <param name="uAccessPwd">4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：bit</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：bit</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <param name="uBank">写入数据的bank</param>
    /// <param name="uPtr">写入数据的起始地址， 单位：字</param>
    /// <param name="uCnt">写入数据的长度， 单位：字</param>
    /// <param name="uDatabuf">写入的数据</param>
    /// <returns></returns>
    public bool WriteData(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte uBank, int uPtr, byte uCnt, byte[] uDatabuf)
    {
        if (UHFWriteData(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData, uBank, uPtr, uCnt, uDatabuf) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 写标签到ECP
    /// </summary>
    /// <param name="accessPwd">4字节密码</param>
    /// <param name="filterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="filterPtr">启动过滤的起始地址， 单位：bit</param>
    /// <param name="filterCnt">启动过滤的长度， 单位：bit</param>
    /// <param name="filterData">启动过滤的数据</param>
    /// <param name="writeData">写入的EPC数据</param>
    /// <returns></returns>
    public bool writeDataToEpc(byte[] accessPwd, byte filterBank, int filterPtr, int filterCnt, byte[] filterData, byte[] writeData)
    {
        if (writeData == null || writeData.Length == 0 || (writeData.Length % 2 != 0))
        {
            throw new Exception("The length of the written data must be a multiple of 2.");
        }
        byte[] newWriteData = new byte[writeData.Length + 2];
        newWriteData[0] = (byte)((writeData.Length / 2) << 3);
        newWriteData[1] = 0;
        Array.Copy(writeData, 0, newWriteData, 2, writeData.Length);
        byte cnt = (byte)(newWriteData.Length / 2);

        return WriteData(accessPwd, filterBank, filterPtr, filterCnt, filterData, 1, 1, cnt, newWriteData);
    }


    /// <summary>
    /// LOCK标签
    /// </summary>
    /// <param name="uAccessPwd"> 4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：字节</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：字节</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <param name="lockbuf">3字节，第0-9位为Action位， 第10-19位为Mask位</param>
    /// <returns></returns>
    public bool LockTag(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte[] lockbuf)
    {
        if (UHFLockTag(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData, lockbuf) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    ///  KILL标签
    /// </summary>
    /// <param name="uAccessPwd">4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：字节</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：字节</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <returns></returns>
    public bool KillTag(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData)
    {
        if (UHFKillTag(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData) == 0)
        {

            return true;
        }
        return false;

    }
    /// <summary>
    /// BlockWrite 特定长度的数据到标签的特定地址
    /// </summary>
    /// <param name="uAccessPwd">4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：字节</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：字节</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <param name="uBank">块号  1：EPC, 2:TID, 3:USR</param>
    /// <param name="uPtr">读取数据的起始地址， 单位：字</param>
    /// <param name="uCnt">读取数据的长度， 单位：字</param>
    /// <param name="uDatabuf">写入的数据</param>
    /// <returns></returns>
    public bool BlockWriteData(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte uBank, int uPtr, int uCnt, byte[] uDatabuf)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("\r\nUHFBlockWriteData================>");
        sb.Append("\r\n密码：" + DataConvert.ByteArrayToHexString(uAccessPwd));
        sb.Append("\r\n过滤数据块（ 1：EPC, 2:TID, 3:USR）：" + FilterBank);
        sb.Append("\r\n过滤起始地址：" + FilterStartaddr);
        sb.Append("\r\n过滤长度：" + FilterLen);
        sb.Append("\r\n过滤数据：" + DataConvert.ByteArrayToHexString(FilterData));
        sb.Append("\r\n");
        sb.Append("\r\n写入的数据块：" + uBank);
        sb.Append("\r\n写入的数据起始地址：" + uPtr);
        sb.Append("\r\n写入的数据长度：" + uCnt);
        sb.Append("\r\n写入的数据内容：" + DataConvert.ByteArrayToHexString(uDatabuf));
        sb.Append("\r\n");

        FileManage.WriterFile("C:\\Users\\Administrator\\Desktop\\UHFLog.txt", sb.ToString(), true);

        if (UHFBlockWriteData(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData, uBank, uPtr, uCnt, uDatabuf) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// BlockErase 特定长度的数据到标签的特定地址
    /// </summary>
    /// <param name="uAccessPwd">4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：字节</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：字节</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <param name="uBank">块号  1：EPC, 2:TID, 3:USR</param>
    /// <param name="uPtr">读取数据的起始地址， 单位：字</param>
    /// <param name="uCnt">读取数据的长度， 单位：字</param>
    /// <returns></returns>
    public bool BlockEraseData(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte uBank, int uPtr, byte uCnt)
    {
        if (UHFBlockEraseData(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData, uBank, uPtr, uCnt) == 0)
        {

            return true;
        }
        return false;
    }
    /// <summary>
    /// Block Permalock操作
    /// </summary>
    /// <param name="uAccessPwd">4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：字节</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：字节</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <param name="ReadLock">bit0：（0：表示Read ， 1：表示Permalock）  </param>
    /// <param name="uBank">块号  1：EPC, 2:TID, 3:USR</param>
    /// <param name="uPtr">Block起始地址 ，单位为16个块</param>
    /// <param name="uRange">Block范围，单位为16个块</param>
    /// <param name="uMaskbuf">块掩码数据，2个字节，16bit 对应16个块</param>
    /// <returns></returns>
    public bool BlockPermalock(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte ReadLock, byte uBank, int uPtr, byte uRange, byte[] uMaskbuf)
    {
        if (UHFBlockPermalock(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData, ReadLock, uBank, uPtr, uRange, uMaskbuf) == 0)
        {
            return true;

        }
        return false;
    }
    #endregion

    #region QT 相关
    /// <summary>
    /// 设置QT命令参数
    /// </summary>
    /// <param name="uAccessPwd">4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：字节</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：字节</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <param name="QTData">bit0：（0：表示无近距离控制 ， 1：表示启用近距离控制）  bit1：(0:表示启用private Memory map, 1：启用public memory map)</param>
    /// <returns></returns>
    public bool SetQT(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte QTData)
    {
        if (UHFSetQT(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData, QTData) == 0) { return true; }
        return false;

    }
    /// <summary>
    /// 获取QT命令参数
    /// </summary>
    /// <param name="uAccessPwd">4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：字节</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：字节</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <param name="QTData">bit0：（0：表示无近距离控制 ， 1：表示启用近距离控制）  bit1：(0:表示启用private Memory map, 1：启用public memory map)</param>
    /// <returns></returns>
    public bool GetQT(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, ref byte QTData)
    {
        if (UHFGetQT(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData, ref QTData) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// QT标签读操作
    /// </summary>
    /// <param name="uAccessPwd">4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：字节</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：字节</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <param name="QTData">bit0：（0：表示无近距离控制 ， 1：表示启用近距离控制）  </param>
    /// <param name="uBank">块号  1：EPC, 2:TID, 3:USR</param>
    /// <param name="uPtr">读取数据的起始地址， 单位：字</param>
    /// <param name="uCnt">读取数据的长度， 单位：字</param>
    /// <param name="uReadDatabuf">读出的数据</param>
    /// <param name="uReadDataLen">读出的数据长度</param>
    /// <returns></returns>
    public string ReadQT(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte QTData, byte uBank, int uPtr, byte uCnt)
    {
        byte[] uReadDatabuf = new byte[512];
        byte uReadDataLen = 0;
        if (UHFReadQT(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData, QTData, uBank, uPtr, uCnt, uReadDatabuf, ref uReadDataLen) == 0)
        {
            string strData = DataConvert.ByteArrayToHexString(uReadDatabuf, uReadDataLen); //BitConverter.ToString(uReadDatabuf, 0, uReadDataLen).Replace("-", "");
            return strData;
        }
        return string.Empty;
    }
    /// <summary>
    ///   QT标签写操作
    /// </summary>
    /// <param name="uAccessPwd"> 4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr"> 启动过滤的起始地址， 单位：字节</param>
    /// <param name="FilterLen">启动过滤的长度， 单位：字节</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <param name="QTData">bit0：（0：表示无近距离控制 ， 1：表示启用近距离控制） </param>
    /// <param name="uBank">块号  1：EPC, 2:TID, 3:USR</param>
    /// <param name="uPtr">读取数据的起始地址， 单位：字</param>
    /// <param name="uCnt">读取数据的长度， 单位：字</param>
    /// <param name="uDatabuf">写入的数据</param>
    /// <returns></returns>
    public bool WriteQT(byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, byte QTData, byte uBank, int uPtr, byte uCnt, byte[] uDatabuf)
    {
        if (UHFWriteQT(uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData, QTData, uBank, uPtr, uCnt, uDatabuf) == 0)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region 盘点相关
    /// <summary>
    /// 设置寻标签过滤设置
    /// </summary>
    /// <param name="saveflag">1:掉电保存， 0：不保存</param>
    /// <param name="bank">0x01:EPC , 0x02:TID, 0x03:USR</param>
    /// <param name="startaddr">起始地址，单位：字节</param>
    /// <param name="datalen">数据长度， 单位:字节</param>
    /// <param name="databuf">数据</param>
    /// <returns></returns>
    public bool SetFilter(byte saveflag, byte bank, int startaddr, int datalen, byte[] databuf)
    {
        if (UHFSetFilter(saveflag, bank, startaddr, datalen, databuf) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 单次盘存标签
    /// </summary>
    /// <param name="uLenUii">UII长度</param>
    /// <param name="uUii">UII数据</param>
    /// <returns></returns>
    public bool InventorySingle(ref byte uLenUii, ref byte[] uUii)
    {
        if (UHFInventorySingle(ref uLenUii, uUii) == 0)
            return true;
        return false;
    }
    /// <summary>
    /// 连续盘存标签
    /// </summary>
    /// <returns></returns>
    public bool Inventory()
    {

        int result = UHFInventory();
        if (result == 0)
            return true;
        else
            return false;
    }
    /// <summary>
    /// 停止盘存标签
    /// </summary>
    /// <returns></returns>
    public bool StopGet()
    {

        if (UHFStopGet() == 0)
            return true;
        else
            return false;

    }
    /// <summary>
    /// 获取连续盘存标签数据
    /// </summary>
    /// <param name="uLenUii">UII长度</param>
    /// <param name="uUii">UII数据</param>
    /// <returns></returns>
    public bool GetReceived_EX(ref int uLenUii, ref byte[] uUii)
    {
        if (UHF_GetReceived_EX(ref uLenUii, uUii) == 0)
        {
            return true;
        }
        return false;
    }
    //读取epc
    public bool uhfGetReceived(ref string epc, ref string tid, ref string rssi, ref string ant)
    {
        int uLen = 0;
        byte[] bufData = new byte[256];
        if (GetReceived_EX(ref uLen, ref bufData))
        {
            //  uUii = 1byteUII长度+UII数据+1byteTID数据+TID+2bytesRSSI
            string epc_data = string.Empty;
            string uii_data = string.Empty;//uii数据
            string tid_data = string.Empty; //tid数据
            string rssi_data = string.Empty;
            string ant_data = string.Empty;

            int uii_len = bufData[0];//uii长度
            int tid_leng = bufData[uii_len + 1];//tid数据长度
            int tid_idex = uii_len + 2;//tid起始位
            int rssi_index = 1 + uii_len + 1 + tid_leng;
            int ant_index = rssi_index + 2;

            string strData = BitConverter.ToString(bufData, 0, uLen).Replace("-", "");
            epc_data = strData.Substring(6, uii_len * 2 - 4);  //Epc
            tid_data = strData.Substring(tid_idex * 2, tid_leng * 2); //Tid
            string temp = strData.Substring(rssi_index * 2, 4);
            int rssiTemp = Convert.ToInt32(temp, 16) - 65535;
            rssi_data = ((float)rssiTemp / 10.0).ToString();// RSSI  =  (0xFED6   -65535)/10
            ant_data = Convert.ToInt32((strData.Substring(ant_index * 2, 2)), 16).ToString();

            epc = epc_data;
            tid = tid_data;
            rssi = rssi_data;
            ant = ant_data;
            return true;
        }
        else
        {
            return false;
        }
    }

    public UHFTAGInfo uhfGetReceived()
    {
        int uLen = 0;
        byte[] bufData = new byte[256];
        if (GetReceived_EX(ref uLen, ref bufData))
        {
            string epc_data = string.Empty;
            string uii_data = string.Empty;//uii数据
            string tid_data = string.Empty; //tid数据
            string rssi_data = string.Empty;
            string ant_data = string.Empty;
            string user_data = string.Empty;

            int uii_len = bufData[0];//uii长度
            int tid_leng = bufData[uii_len + 1];//tid数据长度
            int tid_idex = uii_len + 2;//tid起始位
            int rssi_index = 1 + uii_len + 1 + tid_leng;
            int ant_index = rssi_index + 2;

            string strData = BitConverter.ToString(bufData, 0, uLen).Replace("-", "");
            epc_data = strData.Substring(6, uii_len * 2 - 4);  //Epc

            if (tid_leng > 12)
            {
                tid_data = strData.Substring(tid_idex * 2, 24); //Tid
                user_data = strData.Substring(tid_idex * 2 + 24, (tid_leng - 12) * 2); //Tid
            }
            else
            {
                tid_data = strData.Substring(tid_idex * 2, tid_leng * 2); //Tid
                if (tid_data.Length < 8)
                {
                    tid_data = "";
                }
            }

            string temp = strData.Substring(rssi_index * 2, 4);
            int rssiTemp = Convert.ToInt32(temp, 16) - 65535;
            rssi_data = ((float)rssiTemp / 10.0).ToString();// RSSI  =  (0xFED6   -65535)/10
            if (!rssi_data.Contains("."))
            {
                rssi_data = rssi_data + ".0";
            }
            ant_data = Convert.ToInt32((strData.Substring(ant_index * 2, 2)), 16).ToString();

            UHFTAGInfo info = new UHFTAGInfo();
            info.Epc = epc_data;
            info.Tid = tid_data;
            info.Rssi = rssi_data;
            info.Ant = ant_data;
            info.User = user_data;

            return info;
        }
        else
        {
            return null;
        }
    }

    public bool InventorySingle(ref string epc)
    {
        string tid = string.Empty;
        string rssi = string.Empty;
        byte uLen = 0;
        byte[] bufData = new byte[256];
        if (UHFInventorySingle(ref uLen, bufData) == 0)
        {
            //  uUii = 1byteUII长度+UII数据+1byteTID数据+TID+2bytesRSSI
            string epc_data = string.Empty;
            string uii_data = string.Empty;//uii数据
            string tid_data = string.Empty; //tid数据
            string rssi_data = string.Empty;

            // int uii_len = bufData[0];//uii长度
            int epclen = ((bufData[0] >> 3)) * 2;
            // int tid_leng = bufData[uii_len + 1];//tid数据长度
            //  int tid_idex = uii_len + 2;//tid起始位
            // int rssi_index = 1 + uii_len + 1 + tid_leng;

            string strData = BitConverter.ToString(bufData, 0, uLen).Replace("-", "");
            epc_data = strData.Substring(4, epclen * 2);  //Epc
                                                          //   tid_data = strData.Substring(tid_idex * 2, tid_leng * 2); //Tid
                                                          //    string temp = strData.Substring(rssi_index * 2, 4);
                                                          //    rssi_data = ((Convert.ToInt32(temp, 16) - 65535) / 10).ToString();// RSSI  =  (0xFED6   -65535)/10

            epc = epc_data;
            //  tid = tid_data;
            //  rssi = rssi_data;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 设置EPC模式
    /// </summary>
    /// <returns></returns>
    public bool setEPCMode(bool isSave)
    {
        int flag = isSave ? 1 : 0;
        if (UHFSetEPCTIDUSERMode((byte)flag, 0, 0, 0) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 设置EPC+TID模式
    /// </summary>
    /// <returns></returns>
    public bool setEPCAndTIDMode(bool isSave)
    {
        int flag = isSave ? 1 : 0;
        if (UHFSetEPCTIDUSERMode((byte)flag, 0x01, 0, 0) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 设置EPC+TID+USER模式
    /// </summary>
    /// <returns></returns>
    public bool setEPCAndTIDUSERMode(bool isSave, byte userAddress, byte userLenth)
    {
        int flag = isSave ? 1 : 0;
        if (UHFSetEPCTIDUSERMode((byte)flag, 0x02, userAddress, userLenth) == 0)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取盘点模式
    /// </summary>
    /// <param name="userAddress">user区的起始地址</param>
    /// <param name="userLenth">user区的长度</param>
    /// <returns>0:EPC;  1:EPC+TID;  2:EPC+TID:USER</returns>
    public int getEPCTIDUSERMode(ref byte userAddress, ref byte userLenth)
    {
        byte[] mode = new byte[10];
        int result = UHFGetEPCTIDUSERMode(0, 0, mode);
        if (result > 0)
        {
            userAddress = mode[1];
            userLenth = mode[2];
            return mode[0];
        }
        else
        {
            return -1;
        }
    }

    #endregion

    #region EM4124标签
    /// <summary>
    /// 激活或失效EM4124标签
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="uAccessPwd">4字节密码</param>
    /// <param name="FilterBank">启动过滤的bank号， 1：EPC, 2:TID, 3:USR</param>
    /// <param name="FilterStartaddr">启动过滤的起始地址， 单位：bit</param>
    /// <param name="FilterLen">启动过滤的数据长度</param>
    /// <param name="FilterData">启动过滤的数据</param>
    /// <returns></returns>
    public bool Deactivate(int cmd, byte[] uAccessPwd, byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData)
    {
        if (UHFDeactivate(cmd, uAccessPwd, FilterBank, FilterStartaddr, FilterLen, FilterData) == 0)
            return true;
        else
            return false;

    }

    #endregion

    #region 工作模式
    /// <summary>
    /// 工作模式
    /// </summary>
    /// <param name="mode">0:命令工作模式   1:自动工作模式   2:触发模式</param>
    /// <returns></returns>
    public bool SetWorkMode(byte mode)
    {
        if (UHFSetWorkMode(mode) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool GetWorkMode(byte[] mode)
    {
        if (UHFGetWorkMode(mode) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 设置触发工作模式参数
    /// </summary>
    /// <param name="ioControl">触发IO：0x00表示触发输入1；0x01表示触发输入2</param>
    /// <param name="workTime">触发工作时间：表示有触发输入时读卡工作时间，单位是10ms</param>
    /// <param name="intervalTime">触发时间间隔：表示触发工作时间结束后，间隔多久再判断触发输入IO口进行触发</param>
    /// <param name="mode">0x00表示串口输出，0x01表示UDP输出</param>
    /// <returns></returns>
    public bool SetWorkModePara(byte ioControl, int workTime, int intervalTime, byte mode)
    {
        byte[] para = new byte[6];
        para[0] = ioControl;
        para[1] = (byte)((workTime >> 8) & 0xFF);
        para[2] = (byte)(workTime & 0xFF);
        para[3] = (byte)((intervalTime >> 8) & 0xFF);
        para[4] = (byte)(intervalTime & 0xFF);
        para[5] = mode;
        int result = UHFSetWorkModePara(para);
        return result == 0;
    }
    public bool GetWorkModePara(ref byte ioControl, ref int workTime, ref int intervalTime, ref byte mode)
    {
        byte[] para = new byte[6];
        if (UHFGetWorkModePara(para) == 0)
        {
            ioControl = para[0];
            workTime = (para[1] << 8) | (para[2] & 0xFF);
            intervalTime = (para[3] << 8) | (para[4] & 0xFF);
            mode = para[5];
            return true;
        }
        return false;
    }


    #endregion

    #region 过热保护
    /// <summary>
    /// 设置温度过热保护
    /// </summary>
    /// <param name="tempVal">50-75</param>
    /// <returns></returns>
    public bool SetTemperatureVal(byte tempVal)
    {
        if (tempVal < 50 || tempVal > 75)
            return false;

        if (UHFSetTempVal(tempVal) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 获取温度过热保护值
    /// </summary>
    /// <returns></returns>
    public int GetTemperatureVal()
    {
        byte[] tempVal = new byte[5];
        if (UHFGetTempVal(tempVal) == 0)
        {
            return tempVal[0];
        }
        else
        {
            return -1;
        }
    }
    #endregion

    #region GPIO
    /// <summary>
    /// 获取gpio输入
    /// </summary>
    /// <param name="outData">
    ///       outData[0]:    0:低电平   1：高电平
    ///       outData[1]:    0:低电平   1：高电平
    /// 
    /// </param>
    /// <returns></returns>
    public bool getIOControl(byte[] outData)
    {
        byte[] tempVal = new byte[5];
        if (UHFGetIOControl(tempVal) == 0)
        {
            if (outData != null && outData.Length >= 2)
            {
                outData[0] = tempVal[0];
                outData[1] = tempVal[1];
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 设置gpio输出
    /// </summary>
    /// <param name="ouput1">0:低电平   1：高电平</param>
    /// <param name="ouput2">0:低电平   1：高电平</param>
    /// <param name="outStatus">继电器 0：断开    1：闭合</param>
    /// <returns></returns>
    public bool setIOControl(byte ouput1, byte ouput2, byte outStatus)
    {
        if (ouput1 != 0 && ouput1 != 1)
        {
            return false;
        }
        if (ouput2 != 0 && ouput2 != 1)
        {
            return false;
        }
        if (outStatus != 0 && outStatus != 1)
        {
            return false;
        }
        if (UHFSetIOControl(ouput1, ouput2, outStatus) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool SetOutput(byte[] outData)
    {
        int result = UHFSetOutputIO(outData, (byte)outData.Length);
        return result == 0;

    }
    public bool GetInputStatus(byte[] statusData)
    {
        byte[] temp = new byte[10];
        int[] dataLen = new int[1];
        int result = UHFGetIOStatus(temp, dataLen);
        if (result == 0)
        {
            statusData[0] = temp[1];
            statusData[1] = temp[3];
            return true;
        }
        return false;
    }


    #endregion

    #region 工作时间等待时间
    /// <summary>
    /// 设置工作时间和等待時間
    /// </summary>
    /// <param name="workTime">工作時間</param>
    /// <param name="waitTime">等待時間</param>
    /// <param name="isSave">是否保存</param>
    /// <returns></returns>
    public bool setWorkAndWaitTime(int workTime, int waitTime, bool isSave)
    {
        byte work1 = (byte)((workTime >> 8) & 0xFF);
        byte work2 = (byte)(workTime & 0xFF);
        byte wait1 = (byte)((waitTime >> 8) & 0xFF);
        byte wait2 = (byte)(waitTime & 0xFF);

        if (UHFSetWorkTime((byte)(isSave ? 1 : 0), work1, work2, wait1, wait2) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 獲取工作时间和等待時間
    /// </summary>
    /// <param name="workTime">工作時間</param>
    /// <param name="waitTime">等待時間</param>
    /// <param name="isSave">是否保存</param>
    /// <returns></returns>
    public bool getWorkAndWaitTime(out int workTime, out int waitTime)
    {
        byte[] data = new byte[100];
        int result = UHFGetWorkTime(data);

        if (result == -1)
        {
            workTime = -1;
            waitTime = -1;
            return false;
        }

        workTime = (data[1] | (data[0] << 8));
        waitTime = (data[3] | (data[2] << 8));
        return true;
    }
    #endregion

    #region 升级


    public bool jump2Boot(byte flag)
    {
        int reuslt = UHFJump2Boot(flag);
        return reuslt == 0;
    }

    public bool startUpd()
    {
        int reuslt = UHFStartUpd();
        return reuslt == 0;
    }

    public bool updating(byte[] data, int len)
    {
        int reuslt = UHF_Upgrade(data, len);
        return reuslt == 0;
    }

    public bool stopUpdate()
    {
        int reuslt = UHFStopUpdate();
        return reuslt == 0;
    }
    #endregion

    public string GetAPIVersion()
    {
        byte[] time = new byte[40];
        int result = UHFGetVersionCode(time);
        return "Ver" + result + ".0 (" + System.Text.ASCIIEncoding.ASCII.GetString(time, 0, time.Length).Replace("\0", "") + ")";

    }

    public bool WriteScreenBlockData(byte[] pwd, byte sector, short mask_addr, short mask_len, byte[] mask_data, byte type, short w_addr, short w_len, byte[] w_data)
    {

        return UHFWriteScreenBlockData(pwd, sector, mask_addr, mask_len, mask_data, type, w_addr, w_len, w_data) == 0;
    }

    /**************多设备连接**************/
    /***************************************************************************************/

    //获取当前连接的ip信息
    public List<DeviceInfo> LinkGetDeviceInfo()
    {
        byte[] info = new byte[512];
        int resultLen = LinkGetInfo(info, info.Length);
        if (resultLen > 0)
        {
            string jsonstring = System.Text.ASCIIEncoding.ASCII.GetString(info, 0, resultLen).Replace("\0", "");
            List<DeviceInfo> list = new List<DeviceInfo>();
            object obj = JsonConvert.DeserializeObject(jsonstring);
            JArray arr = JArray.FromObject(obj); //(JArray)JsonConvert.DeserializeObject(jsonstring);


            for (int k = 0; k < arr.Count; k++)
            {
                object _id = arr[k]["id"];
                object _type = arr[k]["type"];
                object _ip = arr[k]["ip"];
                object _port = arr[k]["port"];
                DeviceInfo deviceInfo = new DeviceInfo();
                if (_id != null)
                {
                    deviceInfo.Id = int.Parse(_id.ToString());
                }
                if (_type != null)
                {
                    deviceInfo.Type = _type.ToString();
                }
                if (_port != null)
                {
                    deviceInfo.Port = int.Parse(_port.ToString());
                }
                if (_ip != null)
                {
                    deviceInfo.Ip = _ip.ToString();
                }
                list.Add(deviceInfo);
            }
            return list;

        }
        return null;
    }
    //选择要操作的id，根据LinkGetInfo获取id信息
    public bool LinkSelectDevice(int id)
    {
        return LinkSelect(id) == 0;
    }
    //获取当前可以操作的id
    public int LinkGetSelectedDevice()
    {
        return LinkGetSelected();
    }

    public void LinkDisConnectAllDevice()
    {
        LinkCloseAll();
    }

    public bool InventoryById(int id)
    {
        return UHFInventoryById(id) == 0;
    }
    public bool StopById(int id)
    {
        return UHFStopById(id) == 0;
    }



    public int CalibrationVoltage()
    {
        int[] value = new int[1];
        if (UHFVerifyVoltage(value) == 0)
        {
            int temp = value[0];
            return temp;
        }
        return -1;

    }


    // #define CONTENT_TYPE_INVALID        0
    // #define CONTENT_TYPE_EPC            1
    // #define CONTENT_TYPE_TID            2
    // #define CONTENT_TYPE_USER           3
    // #define CONTENT_TYPE_RSSI           4
    // #define CONTENT_TYPE_ANT            5
    // #define CONTENT_TYPE_ID             6
    // #define CONTENT_TYPE_IP             7
    // #define CONTENT_TYPE_SENSOR         8 
    //return 0,no data, > 0 tag length, < 0 error code
    //tdata tag data, type+length+content+...+type+length+content
    //type:1-epc,2-tid,3-user,4-rssi,5-antenna,6-id
    //06 02 00 00     01 0E 30 00 E2 00 51 57 88 18 01 61 22 20 2F 60 04 02 FD B1 05 01 01
    public TagInfo getTagData()
    {
        TagInfo info = new TagInfo();
        byte[] tagTempData = new byte[150]; //Array.Clear(tagTempData, 0, tagTempData.Length);
        int result = UHFGetTagData(tagTempData, tagTempData.Length);
        info.ErrCode = result;
        if (result > 0)
        {
            // if (tagTempData[0] == 0)
            {
                string hex = BitConverter.ToString(tagTempData, 0, result);
                // Console.WriteLine("hex=" + hex + " result=" + result);
            }
            int index = 0;
            UHFTAGInfo uhfinfo = new UHFTAGInfo();
            while (true)
            {
                if (index > result)
                {
                    break;
                }
                int type = tagTempData[index];
                index = index + 1;
                if (index > result)
                {
                    break;
                }
                int len = tagTempData[index];
                index = index + 1;
                if (index + len > result)
                {
                    break;
                }
                byte[] data = Utils.CopyArray<byte>(tagTempData, index, len);
                index = index + len;

                if (type == 1)
                {
                    //epc
                    uhfinfo.Epc = BitConverter.ToString(data, 2, data.Length - 2).Replace("-", "");
                }
                else if (type == 2)
                {
                    //tid
                    uhfinfo.Tid = BitConverter.ToString(data, 0, data.Length).Replace("-", "");
                }
                else if (type == 3)
                {
                    //user
                    uhfinfo.User = BitConverter.ToString(data, 0, data.Length).Replace("-", "");
                }
                else if (type == 4)
                {
                    //rssi
                    int rssiTemp = (data[1] | (data[0] << 8)) - 65535;
                    string rssi_data = ((float)rssiTemp / 10.0).ToString();// RSSI  =  (0xFED6   -65535)/10
                    if (!rssi_data.Contains("."))
                    {
                        rssi_data = rssi_data + ".0";
                    }
                    uhfinfo.Rssi = rssi_data;
                }
                else if (type == 5)
                {
                    //ant
                    uhfinfo.Ant = data[0].ToString();
                }
                else if (type == 6)
                {
                    //id
                    info.Id = data[1];
                }
                else if (type == 8)
                {
                    //Sensor
                    uhfinfo.Sensor = BitConverter.ToString(data, 0, data.Length).Replace("-", "");
                }
            }
            info.UhfTagInfo = uhfinfo;

        }
        return info;
    }


    #region 温度标签
    public bool InitRegFile(byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData)
    {
        int result = UHFInitRegFile(FilterBank, FilterStartaddr, FilterLen, FilterData);
        if (result == 0)
        {
            return true;
        }
        return false;
    }
    public bool ReadTagTemperature(byte FilterBank, int FilterStartaddr, int FilterLen, byte[] FilterData, float[] outtemp)
    {
        int result = UHFReadTagTemp(FilterBank, FilterStartaddr, FilterLen, FilterData, outtemp);
        if (result == 0)
        {
            return true;
        }
        return false;
    }

    public bool SetDebug(bool debug)
    {
        return SetLogLevel(debug ? 3 : 0) == 0;
    }

    public bool SaveLog(bool debug)
    {
        return SaveLogFile(debug ? 1 : 0) == 0;
    }

    public void StartInventory()
    {
        UHFInventory();
    }

    public void StopInventory()
    {
        UHFStopGet();
    }

    #endregion 温度标签end

}
