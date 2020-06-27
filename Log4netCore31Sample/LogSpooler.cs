/*
*
* LogSpooler.cs
*
* Copyright 2016 Yuichi Yoshii
*     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using log4net.Config;

namespace Logging
{
    public class LogSpooler
    {
        private readonly List<string> errorLogLines;
        private readonly List<string> infoLogLines;
        private readonly List<string> warnLogLines;
        private ILog logClient;
        private int safeTicks;

        private int ticks;

        private Timer timer;

        public LogSpooler()
        {
            var r = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(r, new FileInfo(@"./SaveLog.config"));

            errorLogLines = new List<string>();
            warnLogLines = new List<string>();
            infoLogLines = new List<string>();

            ticks = 0;
            safeTicks = 0;
        }

        public void SetSafe(int arg)
        {
            safeTicks = arg;
        }

        public void Start()
        {
            var callback = new TimerCallback(onUpdate);
            timer = new Timer(callback, null, 1000, 1000);
        }

        private void onUpdate(object arg)
        {
            FlushError();
            FlushWarn();
            FlushInfo();
            ticks++;
            Console.WriteLine(@"LogOperator#onUpdate on Tick " + ticks);
            Console.WriteLine(@"SafeTicks " + safeTicks);
            if (ticks >= safeTicks) Dispose();
        }

        private void FlushError()
        {
            if (errorLogLines.Count > 0)
            {
                var message = new StringBuilder();
                message.AppendLine();
                var lastIndex = errorLogLines.Count - 1;
                if (lastIndex > 0)
                {
                    for (var i = 0; i < lastIndex; i++) message.AppendLine(errorLogLines[i]);
                    logClient = LogManager.GetLogger(Assembly.GetCallingAssembly(), @"ErrorLog");
                    logClient.Error(message.ToString());
                    errorLogLines.RemoveRange(0, lastIndex);
                }
            }
        }

        private void FlushWarn()
        {
            if (warnLogLines.Count > 0)
            {
                var message = new StringBuilder();
                message.AppendLine();
                var lastIndex = warnLogLines.Count - 1;
                if (lastIndex > 0)
                {
                    for (var i = 0; i < lastIndex; i++) message.AppendLine(warnLogLines[i]);
                    logClient = LogManager.GetLogger(Assembly.GetCallingAssembly(), @"WarnLog");
                    logClient.Warn(message.ToString());
                    warnLogLines.RemoveRange(0, lastIndex);
                }
            }
        }

        private void FlushInfo()
        {
            if (infoLogLines.Count > 0)
            {
                var message = new StringBuilder();
                message.AppendLine();
                var lastIndex = infoLogLines.Count - 1;
                if (lastIndex > 0)
                {
                    for (var i = 0; i < lastIndex; i++) message.AppendLine(infoLogLines[i]);
                    logClient = LogManager.GetLogger(Assembly.GetCallingAssembly(), @"InfoLog");
                    logClient.Info(message.ToString());
                    infoLogLines.RemoveRange(0, lastIndex);
                }
            }
        }

        public void Dispose()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            FlushError();
            FlushWarn();
            FlushInfo();
        }

        public void AppendError(string message)
        {
            try
            {
                errorLogLines.Add(message);
            }
            catch (Exception ex)
            {
                logClient = LogManager.GetLogger(Assembly.GetCallingAssembly(), @"LogOperatorLog");
                logClient.Error(@"Exception during AppendError\r\n" + message, ex);
            }
        }

        public void AppendWarn(string message)
        {
            try
            {
                warnLogLines.Add(message);
            }
            catch (Exception ex)
            {
                logClient = LogManager.GetLogger(Assembly.GetCallingAssembly(), @"LogOperatorLog");
                logClient.Error(@"Exception during AppendWarn\r\n" + message, ex);
            }
        }

        public void AppendInfo(string message)
        {
            try
            {
                infoLogLines.Add(message);
            }
            catch (Exception ex)
            {
                logClient = LogManager.GetLogger(Assembly.GetCallingAssembly(), @"LogOperatorLog");
                logClient.Error(@"Exception during AppendInfo\r\n" + message, ex);
            }
        }
    }
}