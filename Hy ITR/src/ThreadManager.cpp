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
		mIsThreadGood = true;
		mThreadError = std::exception();
		mThread = std::jthread(threadWrapper, shared_from_this());
		return true;
	}

	void ThreadManager::restart()
	{
		stop();
		start();
	}

	bool ThreadManager::isRunning() const
	{
		return mThread.joinable();
	}

	bool ThreadManager::isThreadGood() const
	{
		return mIsThreadGood;
	}

	std::string ThreadManager::getError()
	{
		std::lock_guard lock(mThreadLock);
		return std::string(mThreadError.what());
	}

	void ThreadManager::threadWrapper(std::stop_token stoken, std::shared_ptr<ThreadManager> owner) noexcept
	{
		try
		{
			owner->threadFunction(stoken);
		}
		catch (const std::exception& e)
		{
			owner->mIsThreadGood = false;
			std::lock_guard lock(owner->mThreadLock);
			owner->mThreadError = e;
		}
		catch (...)
		{
			owner->mIsThreadGood = false;
			std::lock_guard lock(owner->mThreadLock);
			owner->mThreadError = std::runtime_error("unknown error");
		}
	}
}