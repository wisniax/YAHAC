#include "TestThread.hpp"

using std::cout, std::endl;

namespace hyitr
{
	void hyitr::TestThread::threadFunction(std::stop_token& stoken)
	{
		auto lastStamp = std::chrono::high_resolution_clock::now();
		int to_erase = 0;
		std::chrono::milliseconds stoper(0);
		std::string preStr = "HYITR>Time Elapsed: ";
		std::string fullStr;

		cout << "HYITR>Hello World from Hy ITR Thread!" << endl;

		while (!stoken.stop_requested())
		{
			while (to_erase > 0)
			{
				cout << "\r";
				to_erase--;
			}

			stoper += std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::high_resolution_clock::now() - lastStamp);
			lastStamp = std::chrono::high_resolution_clock::now();
			
			boost::format fmt;
			fullStr = (boost::format("%1% %.2f s") % preStr % (stoper.count() / 1000.f)).str();
			//fullStr = preStr + std::to_string(stoper.count() / 1000) + "." + std::to_string(stoper.count() % 1000) +  " s";
			to_erase = (int)fullStr.length();

			cout << fullStr;
			std::this_thread::sleep_for(std::chrono::milliseconds(10));
		}
		cout << "\nHYITR>Stop requested. Goodbye!" << endl;
	}
	
	
}