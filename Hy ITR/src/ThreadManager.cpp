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
		mIsRunnung = false;
		mThread.request_stop();
		mThread.join();
		return true;
	}

	bool ThreadManager::start()
	{
		if (mThread.joinable())
			return false;
		mIsRunnung = true;
		mIsThreadGood = true;
		mThreadError = "";
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
		return mIsRunnung;
	}

	bool ThreadManager::isThreadGood() const
	{
		return mIsThreadGood;
	}

	std::string ThreadManager::getError()
	{
		std::lock_guard lock(mThreadLock);
		return mThreadError;
	}

	void ThreadManager::threadWrapper(std::stop_token stoken, std::shared_ptr<ThreadManager> owner) noexcept
	{
		try
		{
			owner->threadFunction(stoken);
		}
		catch (const boost::exception& be)
		{
			owner->mIsThreadGood = false;
			owner->mIsRunnung = false;
			std::lock_guard lock(owner->mThreadLock);
			auto formattedError = boost::format("Critical error: %s") % boost::diagnostic_information(be);
			owner->mThreadError = formattedError.str();
		}
		catch (const std::exception& e)
		{
			owner->mIsThreadGood = false;
			owner->mIsRunnung = false;
			std::lock_guard lock(owner->mThreadLock);
			auto formattedError = boost::format("Critical error: %s") % e.what();
			owner->mThreadError = formattedError.str();
		}
		catch (...)
		{
			owner->mIsThreadGood = false;
			owner->mIsRunnung = false;
			std::lock_guard lock(owner->mThreadLock);
			owner->mThreadError = "Unknown critical error";
		}
	}
}