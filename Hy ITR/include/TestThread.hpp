#pragma once
#include <iostream>
#include <string>

#include <sdl2/SDL.h>
#include <sdl2/SDL_vulkan.h>
#include <vulkan/vulkan.hpp>

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