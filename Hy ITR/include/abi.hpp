#pragma once
#include "Version.hpp"

#ifndef HYITR_EXPORT
#define DLLAPI __declspec(dllimport)
#else
#define DLLAPI __declspec(dllexport)
#endif

extern "C"
{
	DLLAPI const char* getVersionInfo();
	DLLAPI const char* helloworld();
}