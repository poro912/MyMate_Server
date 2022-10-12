using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSystem
{
	public delegate void DelIsConnect();
	public class BeforeLoginEvent
	{
		static private event DelIsConnect Connect_check_event;
		static public event DelIsConnect ConnectCheckEvent
		{
			add
			{
				Connect_check_event += value;
			}
			remove
			{
				Connect_check_event -= value;
			}
		}

		static public void ConnectCheck()
		{
			if(Connect_check_event != null)
			{
				Connect_check_event();
			}
		}

	}
}
