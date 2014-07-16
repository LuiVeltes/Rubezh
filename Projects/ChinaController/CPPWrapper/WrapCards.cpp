#include "StdAfx.h"
#include "WrapCards.h"

#include <iostream>
#include <fstream>
using namespace std;

#define QUERY_COUNT	(3)

int CALL_METHOD WRAP_Insert_Card(int loginID, NET_RECORDSET_ACCESS_CTL_CARD* param)
{
	if (NULL == loginID)
	{
		return 0;
	}

	NET_CTRL_RECORDSET_INSERT_PARAM stuInert = {sizeof(stuInert)};
	stuInert.stuCtrlRecordSetInfo.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_IN);
    stuInert.stuCtrlRecordSetInfo.emType = NET_RECORD_ACCESSCTLCARD;
	stuInert.stuCtrlRecordSetInfo.pBuf = param;
	stuInert.stuCtrlRecordSetInfo.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_CARD);
	
	stuInert.stuCtrlRecordSetResult.dwSize = sizeof(NET_CTRL_RECORDSET_INSERT_OUT);
	
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_INSERT, &stuInert, 3000);
	int nRecrdNo = stuInert.stuCtrlRecordSetResult.nRecNo;
	if (bResult)
	{
		return nRecrdNo;
	}
	return 0;
}

BOOL CALL_METHOD WRAP_Update_Card(int loginID, NET_RECORDSET_ACCESS_CTL_CARD* param)
{
	if (NULL == loginID)
	{
		return FALSE;
	}

	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
    stuInert.emType = NET_RECORD_ACCESSCTLCARD;
	stuInert.pBuf = param;
	stuInert.nBufLen = sizeof(NET_RECORDSET_ACCESS_CTL_CARD);
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_UPDATE, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_Remove_Card(int loginID, int recordNo)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.pBuf = &recordNo;
	stuInert.nBufLen = sizeof(recordNo);
	stuInert.emType = NET_RECORD_ACCESSCTLCARD;
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_REMOVE, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_RemoveAll_Cards(int loginID)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	stuInert.emType = NET_RECORD_ACCESSCTLCARD;
    BOOL bResult = CLIENT_ControlDevice(loginID, DH_CTRL_RECORDSET_CLEAR, &stuInert, SDK_API_WAITTIME);
	return bResult;
}

BOOL CALL_METHOD WRAP_Get_Card_Info(int loginID, int recordNo, NET_RECORDSET_ACCESS_CTL_CARD* result)
{
	if (NULL == loginID)
	{
		return FALSE;
	}
	NET_CTRL_RECORDSET_PARAM stuInert = {sizeof(stuInert)};
	NET_RECORDSET_ACCESS_CTL_CARD stuCard = {sizeof(stuCard)};

	stuCard.nRecNo = recordNo;
	stuInert.emType = NET_RECORD_ACCESSCTLCARD;
	stuInert.pBuf = &stuCard;

	int nRet = 0;
	BOOL bRet = CLIENT_QueryDevState(loginID, DH_DEVSTATE_DEV_RECORDSET, (char*)&stuInert, sizeof(stuInert), &nRet, 3000);

	memcpy(result, &stuCard, sizeof(stuCard));
	return bRet;
}

void WRAP_testRecordSetFind_Card(LLONG loginID, LLONG& lFinderId, FIND_RECORD_ACCESSCTLCARD_CONDITION stuParam)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLCARD;

	stuIn.pQueryCondition = &stuParam;
	
	if (CLIENT_FindRecord(loginID, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		lFinderId = stuOut.lFindeHandle;
	}
}

int GetCardsCountRecordSetFind(LLONG& lFinderId)
{
	NET_IN_QUEYT_RECORD_COUNT_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_QUEYT_RECORD_COUNT_PARAM stuOut = {sizeof(stuOut)};
	stuIn.lFindeHandle = lFinderId;
	if (CLIENT_QueryRecordCount(&stuIn, &stuOut, SDK_API_WAITTIME))
	{
		return stuOut.nRecordCount;
	}
	else
	{
		return 0;
	}
}

int CALL_METHOD WRAP_Get_Cards_Count(int loginID)
{
	if (NULL == loginID)
	{
		return -1;
	}
	LLONG lFindID = 0;

	FIND_RECORD_ACCESSCTLCARD_CONDITION stuParam = {sizeof(stuParam)};
	stuParam.bIsValid = TRUE;
	strcpy(stuParam.szCardNo, "1");
	strcpy(stuParam.szUserID, "1");

	WRAP_testRecordSetFind_Card(loginID, lFindID, stuParam);
    if (NULL != lFindID)
    {
		int count = GetCardsCountRecordSetFind(lFindID);
		CLIENT_FindRecordClose(lFindID);
		return count;
    }
	return -1;
}

BOOL CALL_METHOD WRAP_GetAll_Cards2(int loginID, CardsCollection* result)
 {
 	CardsCollection cardsCollection = {sizeof(CardsCollection)};
 
 	LLONG lFinderID = 0;
 
 	FIND_RECORD_ACCESSCTLCARD_CONDITION stuParam = {sizeof(stuParam)};
 	stuParam.bIsValid = TRUE;
 	strcpy(stuParam.szCardNo, "1");
 	strcpy(stuParam.szUserID, "1");
 
 	WRAP_testRecordSetFind_Card(loginID, lFinderID, stuParam);
 	if (lFinderID != 0)
 	{
 		int i = 0, j = 0;
 	
 		NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
 		stuIn.lFindeHandle = lFinderID;
 		stuIn.nFileCount = 10;
 	
 		NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
 		stuOut.nMaxRecordNum = stuIn.nFileCount;
 	
 		NET_RECORDSET_ACCESS_CTL_CARD stuCard[10] = {0};
 		for (i = 0; i < sizeof(stuCard)/sizeof(stuCard[0]); i++)
 		{
 			stuCard[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARD);
 		}
 		stuOut.pRecordList = (void*)&stuCard[0];
 	
 		if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
 		{
 			cardsCollection.Count = stuOut.nRetRecordNum;
 			char szDoorTemp[10][MAX_NAME_LEN] = {0};
 			for (i = 0; i < __min(10, stuOut.nRetRecordNum); i++)
 			{
 				NET_RECORDSET_ACCESS_CTL_CARD* pCard = (NET_RECORDSET_ACCESS_CTL_CARD*)stuOut.pRecordList;
 				memcpy(&cardsCollection.Cards[i], &pCard[i], sizeof(NET_RECORDSET_ACCESS_CTL_CARD));
 			}
 		}

		CLIENT_FindRecordClose(lFinderID);
 	}
 
 	memcpy(result, &cardsCollection, sizeof(CardsCollection));
 	return lFinderID != 0;
 }

BOOL CALL_METHOD WRAP_GetAll_Cards3(int loginID, CardsCollection* result)
{
	CardsCollection cardsCollection = {sizeof(CardsCollection)};
 	LLONG lFinderID = 0;

	NET_IN_FIND_RECORD_PARAM stuIn_1 = {sizeof(stuIn_1)};
	NET_OUT_FIND_RECORD_PARAM stuOut_1 = {sizeof(stuOut_1)};
	stuIn_1.emType = NET_RECORD_ACCESSCTLCARD;
	if (CLIENT_FindRecord(loginID, &stuIn_1, &stuOut_1, SDK_API_WAITTIME))
	{
		lFinderID = stuOut_1.lFindeHandle;
	}

	int index = 0;
	while(true)
	{
		int i = 0, j = 0;
		int nMaxNum = 50000;
		NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
		stuIn.lFindeHandle = lFinderID;
		stuIn.nFileCount = nMaxNum;
	
		NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
		stuOut.nMaxRecordNum = nMaxNum;
	
		NET_RECORDSET_ACCESS_CTL_CARD* pstuCard = new NET_RECORDSET_ACCESS_CTL_CARD[nMaxNum];
		if (NULL == pstuCard)
		{
			return -1;
		}
		memset(pstuCard, 0, sizeof(NET_RECORDSET_ACCESS_CTL_CARD) * nMaxNum);

		for (i = 0; i < nMaxNum; i++)
		{
			pstuCard[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARD);
		}
		stuOut.pRecordList = (void*)pstuCard;
	
		if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
		{
			//if (stuOut.nRetRecordNum > 0)
			//{
			//    ClearResult();
			//}
			for (i = 0; i < __min(stuOut.nMaxRecordNum, stuOut.nRetRecordNum); i++)
			{
				//CString csInfo;
				//csInfo.Format("%d", m_nStartSeq + 1);
	   //         m_nStartSeq++;
				//int nIndex = m_cmbResult.InsertString(-1, csInfo);

				NET_RECORDSET_ACCESS_CTL_CARD* p = new NET_RECORDSET_ACCESS_CTL_CARD;
				if (p != NULL)
				{
					memcpy(p, &pstuCard[i], sizeof(NET_RECORDSET_ACCESS_CTL_CARD));
					memcpy(&cardsCollection.Cards[index], &pstuCard[i], sizeof(NET_RECORDSET_ACCESS_CTL_CARD));
					index++;
					//m_cmbResult.SetItemDataPtr(nIndex, (void*)p);
				}
			}
			//SetDlgItemInt(IDC_RECORDSETFINDER_EDT_RETNUM, stuOut.nRetRecordNum);
		}

		delete[] pstuCard;
		pstuCard = NULL;
		int nRet = stuOut.nRetRecordNum;

		if (nRet <= 0 || nRet > nMaxNum)
		{
			break;
		}
	}

	memcpy(result, &cardsCollection, sizeof(CardsCollection));
 	return lFinderID != 0;
}

BOOL CALL_METHOD WRAP_BeginGetAll_Cards(int loginID, int& finderId)
{
	NET_IN_FIND_RECORD_PARAM stuIn = {sizeof(stuIn)};
	NET_OUT_FIND_RECORD_PARAM stuOut = {sizeof(stuOut)};
	
	stuIn.emType = NET_RECORD_ACCESSCTLCARD;
		
	if (CLIENT_FindRecord(loginID, &stuIn, &stuOut, SDK_API_WAITTIME))
	{
		finderId = stuOut.lFindeHandle;
	}
	return finderId > 0;
}

int CALL_METHOD WRAP_GetAll_Cards(int finderId, CardsCollection* result)
{
	CardsCollection cardsCollection = {sizeof(CardsCollection)};

	int i = 0, j = 0;
	int nMaxNum = 10;
	if (nMaxNum <= 0)
	{
		return -1;
	}
	NET_IN_FIND_NEXT_RECORD_PARAM stuIn = {sizeof(stuIn)};
	stuIn.lFindeHandle = finderId;
	stuIn.nFileCount = nMaxNum;
	
	NET_OUT_FIND_NEXT_RECORD_PARAM stuOut = {sizeof(stuOut)};
	stuOut.nMaxRecordNum = nMaxNum;
	
	NET_RECORDSET_ACCESS_CTL_CARD* pstuCard = new NET_RECORDSET_ACCESS_CTL_CARD[nMaxNum];
	if (NULL == pstuCard)
	{
		return -1;
	}
	memset(pstuCard, 0, sizeof(NET_RECORDSET_ACCESS_CTL_CARD) * nMaxNum);

	for (i = 0; i < nMaxNum; i++)
	{
		pstuCard[i].dwSize = sizeof(NET_RECORDSET_ACCESS_CTL_CARD);
	}
	stuOut.pRecordList = (void*)pstuCard;
	
	if (CLIENT_FindNextRecord(&stuIn, &stuOut, SDK_API_WAITTIME) >= 0)
    {
		for (i = 0; i < __min(stuOut.nMaxRecordNum, stuOut.nRetRecordNum); i++)
		{
			//NET_RECORDSET_ACCESS_CTL_CARD* p = new NET_RECORDSET_ACCESS_CTL_CARD;
			//if (p != NULL)
			{
				//memcpy(p, &pstuCard[i], sizeof(NET_RECORDSET_ACCESS_CTL_CARD));
				memcpy(&cardsCollection.Cards[i], &pstuCard[i], sizeof(NET_RECORDSET_ACCESS_CTL_CARD));
			}
		}
	}

	delete[] pstuCard;
	pstuCard = NULL;

	memcpy(result, &cardsCollection, sizeof(CardsCollection));

	return stuOut.nRetRecordNum;
}

BOOL CALL_METHOD WRAP_EndGetAll_Cards(int finderID)
{
	CLIENT_FindRecordClose(finderID);
    finderID = 0;
	return TRUE;
}

int CALL_METHOD WRAP_GetAllCount(int finderID)
{
    NET_IN_QUEYT_RECORD_COUNT_PARAM stuIn = {sizeof(stuIn)};
    stuIn.lFindeHandle = finderID;
    NET_OUT_QUEYT_RECORD_COUNT_PARAM stuOut = {sizeof(stuOut)};
    if (CLIENT_QueryRecordCount(&stuIn, &stuOut, SDK_API_WAITTIME))
    {
		return stuOut.nRecordCount;
    }
	return -1;
}