#pragma once
#include <iostream>
#include <string>
#include <boost/format.hpp>
#include "ThreadManager.hpp"

namespace hyitr
{
	class TestThread
		: public ThreadManager
	{	
		friend ThreadManager;
	protected:
		TestThread() = default;
	private:
		void threadFunction(std::stop_token& stoken) override;
	};

	
}