using InstrumentServiceBO;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentServiceWS
{
    public partial class InstrumentService : ServiceBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public InstrumentService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger.Warn("Starting Windows Service...");

            QuartzCore core = new QuartzCore();

            core.Start();
        }

        protected override void OnStop()
        {
        }
    }
}
