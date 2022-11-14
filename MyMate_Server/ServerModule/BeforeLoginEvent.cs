using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSystem
{
	// Connect 정보를 삭제하기 위한 델리게이트
	public delegate void DelIsConnect();
	public class BeforeLoginEvent
	{
		static private event DelIsConnect ?Connect_check_event;
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
		// 이벤트를 등록한 모든 객체를 호출하여 연결 상태를 확인한다.
		static public void ConnectCheck()
		{
			// 이벤트에 등록한 객체가 있다면 호출
			if(Connect_check_event != null)
			{
				Connect_check_event();
			}
			// 호출 간소화
			// Connect_check_event?.Invoke();
		}
		async static public void ConnectCheckThread()
		{
			// 1분마다 연결됐는지 확인.
			while(true)
			{
				Console.WriteLine(": 비로그인 클라이언트가 연결 상태인지 확인\n");
				ConnectCheck();
				Thread.Sleep(60000);
			}
		}
	}
}
