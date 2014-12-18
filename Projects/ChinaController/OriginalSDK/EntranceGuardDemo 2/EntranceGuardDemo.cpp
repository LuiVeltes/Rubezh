/********************************************************************
*	Copyright 2013, ZheJiang Dahua Technology Stock Co.Ltd.
* 						All Rights Reserved
*	File Name: EntranceGuardDemo.cpp	    	
*	Author: 
*	Description: 
*	Created:	        %2013%:%12%:%30%  
*	Revision Record:    date:author:modify sth
*********************************************************************/

#include "StdAfx.h"
#include "DevCtrl.h"
#include "DevConfig.h"

using namespace std;

const char* const g_szMenu[] = {
	    "1:DeviceType, SoftwareVer",
		"2:IP,MASK,GATE",
		"3:RecordInsert",           //��¼������
		"4:RecordUpdate",           //��¼������
		"5:RecordDelete",           //��¼��ɾ��
		"6:RecordClear",            //��¼���������
		"7:RecordGet",              //��¼����ȡ
		"8:Reboot",                 //�����豸
		"9:Restore",                //�ָ�����
		"10:DoorOpne",              //����
		"11:GetLogItemCount",       //��ȡ��־������
		"12:GetMAC",                //��ȡ�豸MAC��ַ
		"13:GetAbilityOfRecordFind",//��ȡ��¼����ѯ����
		"14:GetRecordItemCount",    //��ȡ��¼��������
		"15:Get/Set SystemTime",    //��ȡ/�����豸ʱ��
		"16:GetLogItemList",        //��ȡ��־
		"17:AccessCardRecordFind",  //�Ž�����Ϣ��ѯ
		"18:CardPwdRecordFind",     //�Ž�������Ϣ��ѯ
		"19:CardRecordFind",        //ˢ����¼��Ϣ��ѯ
		"20:CfgAccessGeneral",      //�Ž���������
		"21:CfgAccess",             //�Ž���������
		"22:CfgTimeSchedule",       //�Ž�ʱ�������
		"23:DoorClose",             //����
		"24:DoorStatus",            //�Ž�״̬
        "25:CaptureFingerPrint",    //ָ��ץȡ
        "26:OpenDoorGroup_Single",  //�����������,��ͨ��
        "27:OpenDoorGroup_Array",   //�����������,��ͨ��
        "28:OpenDoorRoute_Single",  //����·������,��ͨ��
        "29:OpenDoorRoute_Array",   //����·������,��ͨ��
        "30:ClientCustomData",      //�û��Զ�����������
        "31:Confirm BreakIn",       //BreakIn�¼�ȷ��
};

const char* const g_szRecordSetMenu[] = {
	"1:AccessCard",                 //�ſ���Ϣ
	"2:AccessPwd",                  //�Ž�������Ϣ
	"3:AccessRecord",               //�Ž������¼��Ϣ
	"4:Holiday",                    //������Ϣ
};

// չʾ�˵�
void ShowMenu()
{
	printf("=====TestMenu=====:\n");//�ɲ⹦�ܲ˵�
	for (unsigned int i = 0; i < sizeof(g_szMenu)/sizeof(g_szMenu[0]); i++)
	{
		std::string strTempMenu = g_szMenu[i];
		printf("%s\n", strTempMenu.c_str());
	}
}

// չʾ��¼�������Ӳ˵�
void ShowSubMenu()
{
	printf("=====RecordTypeMenu=====:\n");//��¼�����Ͳ˵�
	for (unsigned int i = 0; i < sizeof(g_szRecordSetMenu)/sizeof(g_szRecordSetMenu[0]); i++)
	{
		std::string strTempMenu = g_szRecordSetMenu[i];
		printf("%s\n", strTempMenu.c_str());
	}
}


// ��ȡ�û�ѡ��
int GetUserChooseFromSubMenu()
{
	ShowSubMenu();
	int nInput = -1;
	do 
	{
		printf("=====��ѡ��: 0.�˳�; 1-%d ��¼���������=====\n", sizeof(g_szRecordSetMenu)/sizeof(g_szRecordSetMenu[0]));
		cin >> nInput;
	} while (0 > nInput || nInput > sizeof(g_szRecordSetMenu)/sizeof(g_szRecordSetMenu[0]));
    return nInput;
}

// ��ȡ�û�ѡ��
int GetUserChooseFromMenu()
{
	ShowMenu();
	int nInput = -1;
	do 
	{
		printf("=====��ѡ��: 0.�˳�; 1-%d ���⹦�����=====\n", sizeof(g_szMenu)/sizeof(g_szMenu[0]));
		cin >> nInput;
	} while (0 > nInput || nInput > sizeof(g_szMenu)/sizeof(g_szMenu[0]));
    return nInput;
}

void CALLBACK DisConnectFunc(LLONG lLoginID, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
	if(dwUser == 0)
	{
		return;
	}
}

int main(int argc, char* argv[])
{
	// ��ʼ��
	unsigned long ulUser = 0;
	printf("=====Init SDK Start=====\n");
    int nInitRet = CLIENT_Init(DisConnectFunc,ulUser);
    printf("CLIENT_Init: %d\n",nInitRet);
	printf("=====Init SDK End  =====\n\n");


	// ��¼
	LLONG lRet = 0;
	printf("=====Login Start========================================\n");
	NET_DEVICEINFO deviceInfo = {0};
	int err = 0;
	while(1)
	{
		//printf("=====Please input IP,Port,Username,Password=====\n");
		//char szIP[25] = {0};
		//int nPort = 0;
		//char szUserName[25] = {0};
		//char szPwd[25] = {0};
		//std::cin >> szIP >> nPort >> szUserName >> szPwd;
		//lRet = CLIENT_Login(szIP,nPort,szUserName,szPwd,&deviceInfo,&err);
		lRet = CLIENT_Login("172.16.6.41",37777,"admin","123456",&deviceInfo,&err);
		std::cout << "CLIENT_Login return code: "<< lRet << " errorCode: "<< err << " byChanNum:" << deviceInfo.byChanNum<< std::endl;
		std::cout << "byDVRType(expect 54(NET_BSC_SERIAL)):" << (int)deviceInfo.byDVRType << std::endl;
		if (NULL == lRet)
		{
			printf("=====Login failed,Login again or not: 0-quit,1-continue;=====\n");
			int nLoginFlag = 0;
			std::cin >> nLoginFlag;
			if (nLoginFlag)
			{
				continue;
			}
			else
			{
				break;
			}
		}
		else
		{
			break;
		}
	}
	printf("=====Login End  ========================================\n\n");
	if (lRet)
    {
		// ����ҵ��
		printf("=====Test Start========================================\n");
		while (1)
		{
			int nChooseID = GetUserChooseFromMenu();
			if (0 == nChooseID)
			{
				break;
			}
			switch(nChooseID)
			{
				case DEVCFG_TYPE_SOFTVERSION:
					DevConfig_TypeAndSoftInfo(lRet);
                    break;
				case DEVCFG_IP_MASK_GATE:
					DevConfig_IPMaskGate(lRet);
					break;
				case DEVCTL_RECORDSET_INSERT:
					DevCtrl_InsertRecordSet(lRet, GetUserChooseFromSubMenu());
					break;
				case DEVCTL_RECORDSET_UPDATE:
					DevCtrl_UpdateRecordSet(lRet, GetUserChooseFromSubMenu());
					break;	
				case DEVCTL_RECORDSET_REMOVE:
					DevCtrl_RemoveRecordSet(lRet, GetUserChooseFromSubMenu());
					break;
				case DEVCTL_RECORDSET_CLEAR:
					DevCtrl_ClearRecordSet(lRet, GetUserChooseFromSubMenu());
					break;
				case DEVCTL_RECORDSET_GET:
					DevCtrl_GetRecordSetInfo(lRet, GetUserChooseFromSubMenu());
					break;
				case DEVCTL_REBOOT:
					DevCtrl_ReBoot(lRet);
					break;
				case DEVCTL_DELETE_CFGFILE:
					DevCtrl_DeleteCfgFile(lRet);
					break;
				case DEVCTL_OPENDOOR:
					DevCtrl_OpenDoor(lRet);
					break;
				case DEVCTL_GETLOGCOUNT:
					DevCtrl_GetLogCount(lRet);
					break;
				case DEVCFG_MAC:
					DevConfig_MAC(lRet);
					break;
				case DEVCAP_RECORDFINDER:
					DevConfig_RecordFinderCaps(lRet);
					break;
				case DEVGET_RECORDCOUNT:
					DevCtrl_GetRecordSetCount(lRet, GetUserChooseFromSubMenu());
					break;
				case DEVCFG_CURRENTTIME:
					DevConfig_CurrentTime(lRet);
					break;
				case DEVFIND_QUERYLOG:
					Dev_QueryLogList(lRet);
					break;
				case DEV_RECORDSET_FIND_CARD:
					testRecordSetFinder_Card(lRet);
					break;
				case DEV_RECORDSET_FIND_PWD:
					testRecordSetFinder_Pwd(lRet);
					break;
				case DEV_RECORDSET_FIND_CARDREC:
					testRecordSetFinder_CardRec(lRet);
					break;
				case DEVCFG_ACCESS_GENERAL:
					DevConfig_AccessGeneral(lRet);
					break;
				case DEVCFG_ACCESS_CONTROL:
					DevConfig_AccessControl(lRet);
					break;
				case DEVCFG_ACCESS_TIMESCHEDULE:
					DevConfig_AccessTimeSchedule(lRet);
					break;
				case DEVCTL_CLOSEDOOR:
					DevCtrl_CloseDoor(lRet);
					break;
				case DEV_DOOR_STATUS:
					DevState_DoorStatus(lRet);
					break;
                case DEVCTL_CAPTURE_FINGERPRINT:
                    DevCtrl_CaptureFingerPrint(lRet);
                    break;
                case DEVCFG_OPEN_DOOR_GROUP_S:
                    DevConfig_OpenDoorGroup_Single(lRet);
                    break;
                case DEVCFG_OPEN_DOOR_GROUP_A:
                    DevConfig_OpenDoorGroup_Array(lRet);
                    break;
                case DEVCFG_OPEN_DOOR_ROUTE_S:
                    DevConfig_OpenDoorRoute_Single(lRet);
                    break;
                case DEVCFG_OPEN_DOOR_ROUTE_A:
                    DevConfig_OpenDoorRoute_Array(lRet);
                    break;
                case DEVCFG_CLIENT_CUSTOM_DATA:
                    DevConfig_ClientCustomData(lRet);
                    break;
                case DEVCTL_CONFIRM_BREAKIN:
                    DevCtrl_Confirm_BreakIn(lRet);
                    break;
				default:
					break;          
			}
			printf("\n=====Press any key to Test Again========================================\n\n");
            getchar();
            getchar();
		}
		printf("=====Test End========================================\n\n");
    }

    // �ǳ�
	if (lRet)
	{
		printf("=====Logout Start=======================================\n");
		BOOL bLogout = CLIENT_Logout(lRet);
		std::cout << "CLIENT_Logout return code: "<< bLogout << std::endl;
		printf("=====Logout End  =======================================\n\n");
	}
	// �ͷ���Դ
	CLIENT_Cleanup();
	printf("=====CLIENT_Cleanup=====\n");
	system("pause");
	return 0;
}

