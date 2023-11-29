#include "BEZ.hpp"

ByteExchangeZoneManager::ByteExchangeZone::ByteExchangeZone(size_t Size)
	: size(Size), ptr(new uint8_t[Size])
{}

ByteExchangeZoneManager::ByteExchangeZone::ByteExchangeZone(ByteExchangeZone&& other)
	: size(other.size), ptr(other.ptr)
{
	other.ptr = nullptr;
}

ByteExchangeZoneManager::ByteExchangeZone& ByteExchangeZoneManager::ByteExchangeZone::operator= (ByteExchangeZone&& other)
{
	size = other.size;
	delete[] ptr;
	ptr = other.ptr;
	return *this;
}

ByteExchangeZoneManager::ByteExchangeZone::~ByteExchangeZone()
{
	delete[] ptr;
	ptr = nullptr;
}

uint8_t* ByteExchangeZoneManager::createBez(size_t size)
{
	mDatabase.push_back(ByteExchangeZone(size));
	return mDatabase.back().ptr;
}

size_t ByteExchangeZoneManager::getBezSize(uint8_t* bez_ptr) const
{
	auto bez = findBez(bez_ptr);
	if (bez == mDatabase.cend())
		return 0;
	return bez->size;
}

void ByteExchangeZoneManager::destroyBez(uint8_t* bez_ptr)
{
	auto bez = findBez(bez_ptr);
	if (bez == mDatabase.cend())
		throw std::runtime_error("invalid BEZ");
	mDatabase.erase(bez);
	bez_ptr = nullptr;
}

bool ByteExchangeZoneManager::isBez(uint8_t* bez_ptr) const
{
	auto bez = findBez(bez_ptr);
	return bez != mDatabase.cend();
}

std::vector<ByteExchangeZoneManager::ByteExchangeZone>::const_iterator ByteExchangeZoneManager::findBez(uint8_t* bez_ptr) const noexcept
{
	auto bez = std::find_if
	(
		mDatabase.cbegin(),
		mDatabase.cend(),
		[bez_ptr](const ByteExchangeZone& element) { return element.ptr == bez_ptr; }
	);
	return bez;
}

