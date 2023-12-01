#pragma once
#include <thread>
#include <memory>
#include <mutex>
#include <type_traits>
#include <boost/exception/all.hpp>
#include <boost/format.hpp>
#include "CommonInterface.hpp"

namespace hyitr
{
	class ThreadManager 
		:	public internal::NotCopyable
		,	public std::enable_shared_from_this< ThreadManager >
	{
	public:
		virtual ~ThreadManager() = default;

		template<class T>
		requires std::is_base_of_v<ThreadManager,T> && (!std::is_abstract_v<T>)
		static std::shared_ptr<T> create()
		{
			//new is used due to protected ctor, make_shared not usable
			auto sptr = std::shared_ptr<T>(new T());
			sptr->start();
			return sptr;
		}

		std::shared_ptr<ThreadManager> getSharedPtr();

		bool stop();
		bool start();
		void restart();
		bool isRunning() const;
		std::string getError(); 
		bool isThreadGood() const;
	protected:
		ThreadManager() = default;
		virtual void threadFunction(std::stop_token& stoken) = 0;

	private:
		static void threadWrapper(std::stop_token stoken, std::shared_ptr<ThreadManager> owner) noexcept;

		std::jthread mThread;
		std::mutex mThreadLock;
		std::atomic<bool> mIsThreadGood;
		std::string mThreadError;
	};
}