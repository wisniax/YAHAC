#include "ThreadManager.hpp"

namespace hyitr
{

	std::shared_ptr<ThreadManager> ThreadManager::getSharedPtr() {
		return shared_from_this();
	}

	bool ThreadManager::stop()
	{
		if (!mThread.joinable())
			return false;

		mThread.request_stop();
		mThread.join();
		return true;
	}

	bool ThreadManager::start()
	{
		if (mThread.joinable())
			return false;
		mThread = std::jthread(threadWrapper, shared_from_this());
		return true;
	}

	void ThreadManager::restart()
	{
		if (mThread.joinable())
		{
			mThread.request_stop();
			mThread.join();
		}
		start();
	}

	bool ThreadManager::isRunning() const
	{
		return mThread.joinable();
	}

	void ThreadManager::threadWrapper(std::stop_token stoken, std::shared_ptr<ThreadManager> owner) noexcept
	{
		try
		{
			owner->threadFunction(stoken);
		}
		catch (const std::exception& e)
		{
			std::lock_guard lock(owner->mThreadLock);
			owner->mThreadError = e;
		}
		catch (...)
		{
			std::lock_guard lock(owner->mThreadLock);
			owner->mThreadError = std::runtime_error("unknown error");
		}
	}
}