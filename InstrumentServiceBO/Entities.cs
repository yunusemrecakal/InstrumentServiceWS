using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentServiceBO
{
    [DataContract]
    public class GetInstrumentListResponse
    {
        public GetInstrumentListResponse()
        {
            list = new List<Instrument>();
        }
        [DataMember]
        public List<Instrument> list;

        [DataMember]
        public string read;
    }
    [DataContract]
    public class Instrument
    {
        /// <summary>
        /// INSTRUMENT NAME - PURE
        /// </summary>
        [DataMember]
        public string i;
        /// <summary>
        /// DISPLAY NAME -CODE
        /// </summary>
        [DataMember]
        public string n;
        /// <summary>
        /// FinInstId
        /// </summary>
        [DataMember]
        public string f;
        /// <summary>
        /// GRUP-EQUITY_TYPE
        /// </summary>
        [DataMember]
        public string g;
        /// <summary>
        /// IS_WARRANT
        /// </summary>
        [DataMember]
        public string v;
        /// <summary>
        /// PRICE_REFERENCE_ID
        /// </summary>
        [DataMember]
        public string r;
        /// <summary>
        /// YESTERDAY_CLOSE_PRICE
        /// </summary>
        [DataMember]
        public decimal y;
        /// <summary>
        /// ASK_PRICE
        /// </summary>
        [DataMember]
        public decimal a;
        /// <summary>
        /// BID_PRICE
        /// </summary>
        [DataMember]
        public decimal b;
        /// <summary>
        /// LAST_PRICE
        /// </summary>
        [DataMember]
        public decimal l;
        /// <summary>
        /// LastUpd
        /// </summary>
        [DataMember]
        public string d;
        /// <summary>
        /// VB_TRANSACTABLE
        /// </summary>
        [DataMember]
        public string t;
        /// <summary>
        /// USE_T1_BALANCE
        /// </summary>
        [DataMember]
        public int ub;
        /// <summary>
        /// STATUS
        /// </summary>
        [DataMember]
        public int st;
        /// <summary>
        /// OUT_OF_MARKET
        /// </summary>
        [DataMember]
        public int om;
        /// <summary>
        /// BOARD_STATU
        /// </summary>
        [DataMember]
        public int bs;
        /// <summary>
        /// SHORTFALL
        /// </summary>
        [DataMember]
        public int sf;
        /// <summary>
        /// DESCRIPTION
        /// </summary>
        [DataMember]
        public string desc;
        /// <summary>
        /// MARKET_CODE
        /// </summary>
        [DataMember]
        public string mc;
        /// <summary>
        /// UpperLimit
        /// </summary>
        [DataMember]
        public decimal ul;
        /// <summary>
        /// LowerLimit
        /// </summary>
        [DataMember]
        public decimal ll;
        /// <summary>
        /// GROUP_CODE
        /// </summary>
        [DataMember]
        public string gc;
    }

    [DataContract]
    public class SecurityInfo
    {
        /// <summary>
        /// INSTRUMENT NAME - PURE
        /// </summary>
        [DataMember]
        public string I;
        /// <summary>
        /// DISPLAY NAME -CODE
        /// </summary>
        [DataMember]
        public string N;
        /// <summary>
        /// FIN_INST_ID
        /// </summary>
        [DataMember]
        public string F;
        /// <summary>
        /// UPPER_LIMIT
        /// </summary>
        [DataMember]
        public decimal UpperLimit;
        /// <summary>
        /// LOWER_LIMIT
        /// </summary>
        [DataMember]
        public decimal LowerLimit;
        /// <summary>
        /// PRICE_REFERENCE_ID
        /// </summary>
        [DataMember]
        public string R;
        /// <summary>
        /// ASK_PRICE
        /// </summary>
        [DataMember]
        public decimal A;
        /// <summary>
        /// BID_PRICE
        /// </summary>
        [DataMember]
        public decimal B;
        /// <summary>
        /// LAST_PRICE
        /// </summary>
        [DataMember]
        public decimal L;
        /// <summary>
        /// TRADING_SESSION_DESC
        /// </summary>
        [DataMember]
        public string S;
        /// <summary>
        /// YESTERDAY_CLOSE_PRICE
        /// </summary>
        [DataMember]
        public decimal Y;
        /// <summary>
        /// EQUITY_TYPE
        /// </summary>
        [DataMember]
        public string G;
        /// <summary>
        /// MARKET_CODE
        /// </summary>
        [DataMember]
        public string MC;
        /// <summary>
        /// IS_WARRANT
        /// </summary>
        [DataMember]
        public string V;
        /// <summary>
        /// DESCRIPTION
        /// </summary>
        [DataMember]
        public string D;
        /// <summary>
        /// USE_T1_BALANCE
        /// </summary>
        [DataMember]
        public int UB;
        [DataMember]
        public string LastUpd;
        /// <summary>
        /// STATUS
        /// </summary>
        [DataMember]
        public int ST;
        /// <summary>
        /// OUT_OF_MARKET
        /// </summary>
        [DataMember]
        public int OM;
        /// <summary>
        /// BOARD_STATU
        /// </summary>
        [DataMember]
        public int BS;
        /// <summary>
        /// SHORTFALL
        [DataMember]
        public int SF;
        /// <summary>
        /// GROUP_CODE
        /// </summary>
        [DataMember]
        public string GC;

        /// <summary>
        /// FREE PRICE
        [DataMember]
        public bool FP;
    }
}
