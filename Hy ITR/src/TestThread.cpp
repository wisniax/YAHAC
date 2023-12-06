#include "TestThread.hpp"

using std::cout, std::endl;

namespace hyitr
{

	TestThread::TestThread()
	{
		// for debug, add validation layer for error checks inside vulkan
#ifdef _DEBUG
		vkLayers.push_back("VK_LAYER_KHRONOS_validation");
#endif // _DEBUG
	}

	void TestThread::threadFunction(std::stop_token& stoken)
	{
		cout << "HYITR>Hello World from Hy ITR Thread!" << endl;

		cout << "HYITR>Step 1: Init SDL and create Vulkan Instance" << endl;
		initVulkan();
		cout << "HYITR>Step 2: Create Vulkan Device" << endl;
		createVkDevice();
		cout << "HYITR>Step 3: no step 3 :C" << endl;
		cout << "HYITR>I will stay in akward silence..." << endl;
		SDL_Event sdlEvent;
		while (!stoken.stop_requested() && mIsRunnung)
		{
			while (SDL_PollEvent(&sdlEvent))
			{
				switch (sdlEvent.type)
				{
				case SDL_EventType::SDL_QUIT:
					mIsRunnung = false;
					break;
				}
			}
			std::this_thread::sleep_for(std::chrono::milliseconds(100));
		}
		cout << "\nHYITR>Stop requested. Goodbye!" << endl;
		deinitVulkan();
	}

	void TestThread::initVulkan()
	{
		//SDL init
		if (int result = SDL_Init(SDL_INIT_VIDEO) != 0)
			BOOST_THROW_EXCEPTION(std::runtime_error((boost::format("SDL2 Init falied! Error code: %i") % result).str()));

		//SDL create window
		sdlWindow = SDL_CreateWindow("Hy ITR Erupted", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 640, 480, SDL_WINDOW_VULKAN);

		//App info for vulkan
		VkApplicationInfo appInfo{};
		appInfo.sType = VK_STRUCTURE_TYPE_APPLICATION_INFO;
		appInfo.pApplicationName = "Hy ITR Erupted";
		appInfo.applicationVersion = VK_MAKE_VERSION(0, 0, 1);
		appInfo.pEngineName = "No Engine";
		appInfo.engineVersion = VK_MAKE_VERSION(0, 0, 1);
		appInfo.apiVersion = VK_API_VERSION_1_0;

		//Query what vulkan extensions the SDL needs
		uint32_t sdlExtensionCount;
		SDL_Vulkan_GetInstanceExtensions(sdlWindow, &sdlExtensionCount, nullptr);
		std::vector<const char*> sdlExtensions(sdlExtensionCount);
		SDL_Vulkan_GetInstanceExtensions(sdlWindow, &sdlExtensionCount, sdlExtensions.data());

		// validation layer validation
		uint32_t layerCount;
		vkEnumerateInstanceLayerProperties(&layerCount, nullptr);
		std::vector<VkLayerProperties> availableLayers(layerCount);
		vkEnumerateInstanceLayerProperties(&layerCount, availableLayers.data());
		for (auto layer : vkLayers)
		{
			if (
				std::ranges::find_if
				(
					availableLayers,
					[layer](const VkLayerProperties& a)
					{
						return std::strcmp(a.layerName, layer) == 0;
					}
				)
				== availableLayers.cend()
						)
				BOOST_THROW_EXCEPTION(std::runtime_error((boost::format("Validation layer creation error: \"%s\" not found!") % layer).str()));
		}


		//instance info for vulkan
		VkInstanceCreateInfo createInfo{};
		createInfo.sType = VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO;
		createInfo.pApplicationInfo = &appInfo;
		createInfo.enabledExtensionCount = sdlExtensionCount;
		createInfo.ppEnabledExtensionNames = sdlExtensions.data();
		createInfo.enabledLayerCount = (uint32_t)vkLayers.size();
		createInfo.ppEnabledLayerNames = vkLayers.data();
		createInfo.enabledLayerCount = 0;

		//finally create vulkan instance
		if (auto vkResult = vkCreateInstance(&createInfo, nullptr, &vkInstance) != VkResult::VK_SUCCESS)
			BOOST_THROW_EXCEPTION(std::runtime_error("vkCreateInstance error:" + std::to_string(vkResult)));
	}

	void TestThread::createVkDevice()
	{
		uint32_t vkDevicesCount = 0;
		vkEnumeratePhysicalDevices(vkInstance, &vkDevicesCount, nullptr);
		std::vector<VkPhysicalDevice> vkDevices(vkDevicesCount);
		vkEnumeratePhysicalDevices(vkInstance, &vkDevicesCount, vkDevices.data());

		std::cout << "\nDevices with Vulkan support:\n";

		std::optional<VkPhysicalDevice> vkChoosenDevice;
		std::optional<uint32_t> vkChoosenQueueFamily;
		for (const auto& dev : vkDevices)
		{
			vkChoosenDevice = dev;

			VkPhysicalDeviceProperties devProperties{};
			vkGetPhysicalDeviceProperties(dev, &devProperties);
			VkPhysicalDeviceFeatures devFeatures{};
			vkGetPhysicalDeviceFeatures(dev, &devFeatures);

			std::string messageDevType;
			switch (devProperties.deviceType)
			{
			
			case VK_PHYSICAL_DEVICE_TYPE_INTEGRATED_GPU:
				messageDevType = "Integrated GPU";
				break;
			case VK_PHYSICAL_DEVICE_TYPE_DISCRETE_GPU:
				messageDevType = "Discrete GPU";
				break;
			case VK_PHYSICAL_DEVICE_TYPE_VIRTUAL_GPU:
				messageDevType = "Virtual GPU";
				break;
			case VK_PHYSICAL_DEVICE_TYPE_CPU:
				messageDevType = "CPU";
				break;
			default:
				messageDevType = "N/A (Other type)";
				break;
			}

			std::string messageQueueFamilies;
			uint32_t vkQueueFamiliesCount = 0;
			vkGetPhysicalDeviceQueueFamilyProperties(dev, &vkQueueFamiliesCount, nullptr);
			std::vector<VkQueueFamilyProperties> vkQueueFamilies(vkQueueFamiliesCount);
			vkGetPhysicalDeviceQueueFamilyProperties(dev, &vkQueueFamiliesCount, vkQueueFamilies.data());
			int vkQueueFamilyIdx = -1;
			for (const auto& queueFamily : vkQueueFamilies)
			{
				vkQueueFamilyIdx++;
				if (!(queueFamily.queueFlags & VK_QUEUE_GRAPHICS_BIT))
					continue;
				vkChoosenQueueFamily = vkQueueFamilyIdx;
				messageQueueFamilies += (boost::format("\n-->Idx: %i\n--->Count: %i") % vkQueueFamilyIdx % queueFamily.queueCount).str();
			}


			auto message = boost::format("\nDevice: %s\n->Type: %s\n->Queue Families: %s\n->Is suitable?: %s\n")
				% devProperties.deviceName
				% messageDevType
				% messageQueueFamilies
				% (vkChoosenQueueFamily.has_value() ? "YES" : "NOPE");

			std::cout << message;
		}

		if(!vkChoosenDevice.has_value() || !vkChoosenQueueFamily.has_value())
				BOOST_THROW_EXCEPTION(std::runtime_error("No suitable device found!"));

		float vkQueuePriority = 1.0f;
		VkDeviceQueueCreateInfo vkQueueCreateInfo{};
		vkQueueCreateInfo.sType = VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO;
		vkQueueCreateInfo.queueFamilyIndex = vkChoosenQueueFamily.value();
		vkQueueCreateInfo.queueCount = 1;
		vkQueueCreateInfo.pQueuePriorities = &vkQueuePriority;

		VkPhysicalDeviceFeatures deviceFeatures{};

		VkDeviceCreateInfo vkDeviceCreateInfo{};
		vkDeviceCreateInfo.sType = VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO;
		vkDeviceCreateInfo.pQueueCreateInfos = &vkQueueCreateInfo;
		vkDeviceCreateInfo.queueCreateInfoCount = 1;
		vkDeviceCreateInfo.pEnabledFeatures = &deviceFeatures;
		//for compatibility
		vkDeviceCreateInfo.enabledLayerCount = (uint32_t)vkLayers.size();
		vkDeviceCreateInfo.ppEnabledLayerNames = vkLayers.data();

		if (auto vkResult = vkCreateDevice(vkChoosenDevice.value(),&vkDeviceCreateInfo,nullptr,&vkDevice) != VkResult::VK_SUCCESS)
			BOOST_THROW_EXCEPTION(std::runtime_error("vkCreateDevice error:" + std::to_string(vkResult)));
	}
	
	void TestThread::deinitVulkan()
	{
		vkDestroyDevice(vkDevice, nullptr);
		vkDestroyInstance(vkInstance, nullptr);
		SDL_DestroyWindow(sdlWindow);
	}
	
}