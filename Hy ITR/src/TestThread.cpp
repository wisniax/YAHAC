#include "TestThread.hpp"

using std::cout, std::endl;

namespace hyitr
{
	void hyitr::TestThread::threadFunction(std::stop_token& stoken)
	{
		int result = 0;

		cout << "HYITR>Hello World from Hy ITR Thread!" << endl;

		while (!stoken.stop_requested())
		{
			result = SDL_Init(SDL_INIT_VIDEO);
			if (result != 0)
				BOOST_THROW_EXCEPTION(std::runtime_error("SDL Init failed"));

			auto sdlWindow = SDL_CreateWindow("Hy ITR Erupted", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 640, 480, SDL_WINDOW_VULKAN);

			VkInstance instance;

			VkApplicationInfo appInfo{};
			appInfo.sType = VK_STRUCTURE_TYPE_APPLICATION_INFO;
			appInfo.pApplicationName = "Hy ITR Erupted";
			appInfo.applicationVersion = VK_MAKE_VERSION(0, 0, 1);
			appInfo.pEngineName = "No Engine";
			appInfo.engineVersion = VK_MAKE_VERSION(0, 0, 1);
			appInfo.apiVersion = VK_API_VERSION_1_0;

			VkInstanceCreateInfo createInfo{};
			createInfo.sType = VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO;
			createInfo.pApplicationInfo = &appInfo;
			
			uint32_t sdlExtensionCount;
			const char** sdlExtensions;
			SDL_Vulkan_GetInstanceExtensions(sdlWindow, &sdlExtensionCount, nullptr);
			sdlExtensions = new const char*[sdlExtensionCount];
			SDL_Vulkan_GetInstanceExtensions(sdlWindow, &sdlExtensionCount, sdlExtensions);

			createInfo.enabledExtensionCount = sdlExtensionCount;
			createInfo.ppEnabledExtensionNames = sdlExtensions;
			createInfo.enabledLayerCount = 0;

			if (auto vkResult = vkCreateInstance(&createInfo, nullptr, &instance) != VkResult::VK_SUCCESS)
				BOOST_THROW_EXCEPTION(std::runtime_error("vkCreateInstance error:" + std::to_string(vkResult)));

			cout << "HYITR>I will stay in akward silence for 10s ..." << endl;
			std::this_thread::sleep_for(std::chrono::seconds(10));
			cout << "HYITR>Well, my job done! I shall die!!!" << endl;

			vkDestroyInstance(instance, nullptr);
			SDL_DestroyWindow(sdlWindow);
			break;
		}
		cout << "\nHYITR>Stop requested. Goodbye!" << endl;
	}
	
	
}