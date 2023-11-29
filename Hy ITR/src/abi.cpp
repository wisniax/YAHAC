#include "abi.hpp"

#include <string>

//ABI VARS

static ByteExchangeZoneManager bezMan;
static std::shared_ptr<hyitr::TestThread> testTh;

//ABI internal

void abii_str2bez(const std::string& std_str, uint8_t* bez)
{
    memcpy_s(bez, bezMan.getBezSize(bez), std_str.c_str(), std_str.length() + 1);
}

void abii_str2bez_new(const std::string& std_str, uint8_t** bez)
{
    *bez = bezMan.createBez(std_str.length() + 1);
    memcpy_s(*bez, bezMan.getBezSize(*bez), std_str.c_str(), std_str.length() + 1);
}

std::string abii_bez2str(uint8_t* bez)
{
    std::string str((char*)bez);
    return str;
}

//ABI UTILS

uint8_t* newBez(uint64_t size) 
{
    return bezMan.createBez(size);
}

uint64_t getBezSize(uint8_t* bez)
{
    return bezMan.getBezSize(bez);
}

void deleteBez(uint8_t* bez)
{
    bezMan.destroyBez(bez);
}

//ABI
const char* getVersionInfo()
{
    return HYITR_VERSION;
}

const char* helloworld() {
    const char* hello_world = "Hello World, from Hy ITR!";
    return hello_world;
}

void spawnThread()
{
    testTh = hyitr::ThreadManager::create<hyitr::TestThread>();
}

void stopThread()
{
    testTh->stop();
}

bool isThreadRunning()
{
    return testTh->isRunning();
}

uint8_t* getThreadError()
{
    uint8_t* bez = nullptr;
    if(testTh->isThreadGood())
        abii_str2bez_new("", &bez);
    else
        abii_str2bez_new(testTh->getError(), &bez);
    return bez;
}

