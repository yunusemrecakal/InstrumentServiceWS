using Gtp.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentServiceBO
{
    public static class Talker
    {
        private static readonly string partyId = ConfigurationManager.AppSettings["partyId"].ToString();
        private static readonly string employeeId = ConfigurationManager.AppSettings["empId"].ToString();
        private static readonly string positionId = ConfigurationManager.AppSettings["posId"].ToString();
        private static readonly string divisionId = ConfigurationManager.AppSettings["divisonId"].ToString();
        private static readonly string organizationId = ConfigurationManager.AppSettings["OrgId"].ToString();
        private static readonly string organizationGroupId = ConfigurationManager.AppSettings["orgGroupId"].ToString();
        private static readonly string channelId = ConfigurationManager.AppSettings["ChannelId"].ToString();
        private static readonly Invoker invoker = new Invoker(ConfigurationManager.AppSettings.Get("RequestBrokerUri"));

        public static GtpDataSet GetInstrumentList()
        {
            try
            {
                GtpXml G = new GtpXml("FSI_GET_SECURITY_DEFINITIONS", "1.1");
                G.AddParameter("IncludeDetails", "bool", true); // DESC VE PRICE ALANLARI GELMESI SAGLANIR
                G.AddParameter("IncludeStatusInfo", "bool", true);//GTPFSI_EQ_CHTS -> STATUS , OUT_OF_MARKET , BOARD_STATU kolonları gelmesini sağlar
                G.AddParameter("IncludePriceReference", "bool", false); // PRICE_REFERENCE LİSTESI GELMEYECECEK
                G.AddParameter("UseInstantPrice", "bool", true); // FI_VALUES FIYAT ALMASINI SAGLIYOR
                G = RbmInvoke(G);

                return G.GetResponseOutputGtpDataSet("SecurityDefinitions");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static GtpCalendar GetGtpCalendar()
        {
            GtpCalendar response = new GtpCalendar();
            try
            {
                GtpXml G = new GtpXml("CALENDAR_FUNCTIONS", "1.0");
                G.AddParameter("Function", "GetAllData");
                G.AddParameter("MarketPlaceId", "0000-000001-MPL");
                G = RbmInvoke(G);

                response.ActiveSession = G.GetResponseOutputInt("ActiveSession");
                response.AvailableBusinessDay = G.GetResponseOutputDateTime("AvailableBusinessDay");
                response.AvailableSession = G.GetResponseOutputInt("AvailableSession");
                response.BusinessDay = G.GetResponseOutputBoolean("BusinessDay");
                response.CurrentBussinessDate = G.GetResponseOutputDateTime("Date");
                response.DaysToNextBusinessDay = G.GetResponseOutputInt("DaysToNextBusinessDay");
                response.NextBusinessDay = G.GetResponseOutputDateTime("NextBusinessDay");
                response.PreviousBusinessDay = G.GetResponseOutputDateTime("PreviousBusinessDay");

                if (G.HasResponseOutput("Session1Start"))
                    response.Session1Start = G.GetResponseOutputInt("Session1Start");
                if (G.HasResponseOutput("Session1End"))
                    response.Session1End = G.GetResponseOutputInt("Session1End");
                if (G.HasResponseOutput("Session2Start"))
                    response.Session2Start = G.GetResponseOutputInt("Session2Start");
                if (G.HasResponseOutput("Session2End"))
                    response.Session2End = G.GetResponseOutputInt("Session2End");

                response.SessionCount = G.GetResponseOutputInt("SessionCount");
                response.SettlementDate = G.GetResponseOutputDateTime("SettlementDate");
                response.Today = G.GetResponseOutputDateTime("ToDay");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return response;
        }

        public static GtpXml RbmInvoke(GtpXml G)
        {
            G.AddUserInformation(partyId
                , employeeId
                , positionId
                , divisionId
                , organizationId
                , organizationGroupId
                , channelId);

            GtpXml response;

            try
            {
                response = invoker.RbmInvoke(G);
            }
            catch (SocketException e)
            {
                G.AddException(e);
                G.AddError("tcp", "RequestBroker is unreachable");
                response = G;
            }
            return response;
        }
    }
}
