#pragma once
#include <iostream>
#include <string>
#include <optional>

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
		TestThread();
	private:
		void threadFunction(std::stop_token& stoken) override;

		void initVulkan();
		void createVkDevice();
		void deinitVulkan();

		std::vector<const char*> vkLayers;

		SDL_Window* sdlWindow;
		VkInstance vkInstance;
		VkDevice vkDevice;
	};

	
}