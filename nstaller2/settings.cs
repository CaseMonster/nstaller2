using System;
using System.Collections;

namespace nstaller2
{
    class Settings
    {
        public string bindir        = "NULL";
        public string checkdotnet   = "NULL";
        public string checkagent    = "NULL";
        public string checkav       = "NULL";
        public string checkprobe    = "NULL";
        public string bindotnet     = "NULL";
        public string binagent      = "NULL";
        public string binav86       = "NULL";
        public string binav64       = "NULL";
        public string binprobe      = "NULL";

        public Settings(string s0, string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string s9)
        {
            this.bindir         = s0;
            this.checkdotnet    = s1;
            this.checkagent     = s2;
            this.checkav        = s3;
            this.checkprobe     = s4;
            this.bindotnet      = s5;
            this.binagent       = s6;
            this.binav86        = s7;
            this.binav64        = s8;
            this.binprobe       = s9;
        }
    }
}
