#pragma once
#include<vector>
#include<stdexcept>


class ByteExchangeZoneManager
{
private:
	struct ByteExchangeZone
	{
		ByteExchangeZone(size_t Size);

		ByteExchangeZone(ByteExchangeZone&) = delete;
		ByteExchangeZone& operator= (ByteExchangeZone&) = delete;

		ByteExchangeZone(ByteExchangeZone&& other);
		ByteExchangeZone& operator= (ByteExchangeZone&& other);

		~ByteExchangeZone();

		size_t size;
		uint8_t* ptr;
	};

public:
	uint8_t* createBez(size_t size);

	size_t getBezSize(uint8_t* bez_ptr) const;

	void destroyBez(uint8_t* bez_ptr);

	bool isBez(uint8_t* bez_ptr) const;

private:

	std::vector<ByteExchangeZone>::const_iterator findBez(uint8_t* bez_ptr) const noexcept;

	std::vector<ByteExchangeZone> mDatabase;
};
