#pragma once
#include "Version.hpp"
#include "TestThread.hpp"

#ifndef HYITR_EXPORT
#define HYITR_API __declspec(dllimport)
#else
#define HYITR_API __declspec(dllexport)
#endif

extern "C"
{
	HYITR_API const char* getVersionInfo();
	HYITR_API const char* helloworld();
	//Test
	HYITR_API void spawnThread();
	HYITR_API void stopThread();
}