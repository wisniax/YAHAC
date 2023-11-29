#pragma once
#include <vector>
#include "Version.hpp"
#include "BEZ.hpp"
#include "TestThread.hpp"

#ifndef HYITR_EXPORT
#define HYITR_API __declspec(dllimport)
#else
#define HYITR_API __declspec(dllexport)
#endif

extern "C"
{
	//ABI UTILS	

	HYITR_API uint8_t* newBez(uint64_t size);
	HYITR_API uint64_t getBezSize(uint8_t* bez);
	HYITR_API void deleteBez(uint8_t* bez);

	//ABI

	HYITR_API const char* getVersionInfo();
	HYITR_API const char* helloworld();

	//Test
	HYITR_API void spawnThread();
	HYITR_API void stopThread();
	HYITR_API bool isThreadRunning();
	HYITR_API uint8_t* getThreadError();
}