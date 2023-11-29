#pragma once
namespace hyitr::internal
{
	class NotCopyable
	{
	protected:
		NotCopyable() = default;
		~NotCopyable() = default;
	public:
		NotCopyable(NotCopyable&) = delete;
		NotCopyable(NotCopyable&&) = delete;
		NotCopyable& operator=(NotCopyable&) = delete;
		NotCopyable& operator=(NotCopyable&&) = delete;
	};
}