using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CD_Tray
{
    public partial class Form1 : Form
    {
        //DEV LOG
        //App currently functions right out of the box.
        //If no cd tray found, just closes.


        //KEEP APP MANIFEST!!!
        //I HAVE IT THERE TO PREVENT POP-UPS

        //Should work right out of the box though.


        //SETTINGS:

        /// <summary>
        /// Delay in ms between open and closing of tray
        /// </summary>
        public static int SECONDS_BETWEEN_OPEN_AND_CLOSE = 10000;


        /// <summary>
        /// This is the print-screen key.
        /// GO TO LINE 140 FOR LIST OF ALL KEY OFFSETS
        /// </summary>
        public static readonly int CLOSE_KEYBIND = 0x2C;



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.DriveType == System.IO.DriveType.CDRom)
                {
                    CdTray.CdTrayName = drive.Name;
                }
            }
            if(CdTray.CdTrayName == "")
            {
                //Code for if no cd tray found
                //MessageBox.Show("Cd tray could not be found.","WARNING");
                SHUT_DOWN();
            }

            this.Visible = false;
            this.ShowInTaskbar = false;
            Thread threadLoop = new Thread(Loop);
            threadLoop.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //To ensure no threads stay active after closing application
            SHUT_DOWN();
        }


        //Controls main hack loop
        public static bool _running = true;


        //CD Tray Loop
        public static void Loop()
        {
            //Start new thread to listen for PinkKey being pressed
            Thread keyLoop = new Thread(UpdateKeyHook);
            keyLoop.Start();

            //The Loop
            while (_running)
            {
                CdTray.open();
                Thread.Sleep(SECONDS_BETWEEN_OPEN_AND_CLOSE);
                CdTray.close();
                Thread.Sleep(SECONDS_BETWEEN_OPEN_AND_CLOSE);
            }
        }


        //Keybind handler
        public static void UpdateKeyHook()
        {
            //Keybind Stuff
            while (_running)
            {
                if (GetAsyncKeyState(CLOSE_KEYBIND))
                {
                    _running = false;
                    Environment.Exit(0);
                }
                Thread.Sleep(100);
            }
        }


        public static void SHUT_DOWN()
        {
            _running = false;
            Environment.Exit(0);
            Application.Exit();
        }
        [DllImport("User32.dll")]
        private static extern bool GetAsyncKeyState(int vKey);
    }


    public class CdTray
    {
        public static string CdTrayName = "";

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi)]
        protected static extern int mciSendString(string lpstrCommand,StringBuilder lpstrReturnString,int uReturnLength,IntPtr hwndCallback);

        //Open cd tray
        public static void open()
        {
            int ret = mciSendString($"set {CdTrayName} door open", null, 0, IntPtr.Zero);
        }

        //Close cd tray
        public static void close()
        {
            int ret = mciSendString($"set {CdTrayName} door closed", null, 0, IntPtr.Zero);
        }
    }


    /*
Name	Numeric value	Description
VK_ABNT_C1	    0xC1	Abnt C1
VK_ABNT_C2	    0xC2	Abnt C2
VK_ADD	        0x6B	Numpad +
VK_ATTN	        0xF6	Attn
VK_BACK	        0x08	Backspace
VK_CANCEL	    0x03	Break
VK_CLEAR	    0x0C	Clear
VK_CRSEL	    0xF7	Cr Sel
VK_DECIMAL	    0x6E	Numpad .
VK_DIVIDE	    0x6F	Numpad /
VK_EREOF	    0xF9	Er Eof
VK_ESCAPE	    0x1B	Esc
VK_EXECUTE	    0x2B	Execute
VK_EXSEL	    0xF8	Ex Sel
VK_ICO_CLEAR	0xE6	IcoClr
VK_ICO_HELP	    0xE3	IcoHlp
VK_KEY_0	    0x30 ('0')	0
VK_KEY_1	    0x31 ('1')	1
VK_KEY_2	    0x32 ('2')	2
VK_KEY_3	    0x33 ('3')	3
VK_KEY_4	    0x34 ('4')	4
VK_KEY_5	    0x35 ('5')	5
VK_KEY_6	    0x36 ('6')	6
VK_KEY_7	    0x37 ('7')	7
VK_KEY_8	    0x38 ('8')	8
VK_KEY_9	    0x39 ('9')	9
VK_KEY_A	    0x41 ('A')	A
VK_KEY_B	    0x42 ('B')	B
VK_KEY_C	    0x43 ('C')	C
VK_KEY_D	    0x44 ('D')	D
VK_KEY_E	    0x45 ('E')	E
VK_KEY_F	    0x46 ('F')	F
VK_KEY_G	    0x47 ('G')	G
VK_KEY_H	    0x48 ('H')	H
VK_KEY_I	    0x49 ('I')	I
VK_KEY_J	    0x4A ('J')	J
VK_KEY_K	    0x4B ('K')	K
VK_KEY_L	    0x4C ('L')	L
VK_KEY_M	    0x4D ('M')	M
VK_KEY_N	    0x4E ('N')	N
VK_KEY_O	    0x4F ('O')	O
VK_KEY_P	    0x50 ('P')	P
VK_KEY_Q	    0x51 ('Q')	Q
VK_KEY_R	    0x52 ('R')	R
VK_KEY_S	    0x53 ('S')	S
VK_KEY_T	    0x54 ('T')	T
VK_KEY_U	    0x55 ('U')	U
VK_KEY_V	    0x56 ('V')	V
VK_KEY_W	    0x57 ('W')	W
VK_KEY_X	    0x58 ('X')	X
VK_KEY_Y	    0x59 ('Y')	Y
VK_KEY_Z	    0x5A ('Z')	Z
VK_MULTIPLY	    0x6A	Numpad *
VK_NONAME	    0xFC	NoName
VK_NUMPAD0	    0x60	Numpad 0
VK_NUMPAD1	    0x61	Numpad 1
VK_NUMPAD2	    0x62	Numpad 2
VK_NUMPAD3	    0x63	Numpad 3
VK_NUMPAD4	    0x64	Numpad 4
VK_NUMPAD5	    0x65	Numpad 5
VK_NUMPAD6	    0x66	Numpad 6
VK_NUMPAD7	    0x67	Numpad 7
VK_NUMPAD8	    0x68	Numpad 8
VK_NUMPAD9	    0x69	Numpad 9
VK_OEM_1	    0xBA	OEM_1 (: ;)
VK_OEM_102  	0xE2	OEM_102 (> <)
VK_OEM_2	    0xBF	OEM_2 (? /)
VK_OEM_3	    0xC0	OEM_3 (~ `)
VK_OEM_4	    0xDB	OEM_4 ({ [)
VK_OEM_5	    0xDC	OEM_5 (| \)
VK_OEM_6	    0xDD	OEM_6 (} ])
VK_OEM_7	    0xDE	OEM_7 (" ')
VK_OEM_8	    0xDF	OEM_8 (§ !)
VK_OEM_ATTN	    0xF0	Oem Attn
VK_OEM_AUTO	    0xF3	Auto
VK_OEM_AX	    0xE1	Ax
VK_OEM_BACKTAB	0xF5	Back Tab
VK_OEM_CLEAR	0xFE	OemClr
VK_OEM_COMMA	0xBC	OEM_COMMA (< ,)
VK_OEM_COPY	    0xF2	Copy
VK_OEM_CUSEL	0xEF	Cu Sel
VK_OEM_ENLW	    0xF4	Enlw
VK_OEM_FINISH	0xF1	Finish
VK_OEM_FJ_LOYA	0x95	Loya
VK_OEM_JUMP	    0xEA	Jump
VK_OEM_MINUS	0xBD	OEM_MINUS (_ -)
VK_OEM_PA1	    0xEB	OemPa1
VK_OEM_PA2	    0xEC	OemPa2
VK_OEM_PA3	    0xED	OemPa3
VK_OEM_PERIOD	0xBE	OEM_PERIOD (> .)
VK_OEM_PLUS	    0xBB	OEM_PLUS (+ =)
VK_OEM_RESET	0xE9	Reset
VK_OEM_WSCTRL	0xEE	WsCtrl
VK_PA1	        0xFD	Pa1
VK_PACKET	    0xE7	Packet
VK_PLAY	        0xFA	Play
VK_PROCESSKEY	0xE5	Process
VK_RETURN	    0x0D	Enter
VK_SELECT	    0x29	Select
VK_SEPARATOR	0x6C	Separator
VK_SPACE	    0x20	Space
VK_SUBTRACT	    0x6D	Num -
VK_TAB	        0x09	Tab
VK_ZOOM	        0xFB	Zoom
     */
}
