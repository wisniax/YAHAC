#include "abi.hpp"

#include <string>

const char* getVersionInfo()
{
    return HYITR_VERSION;
}

const char* helloworld() {
    const char* hello_world = "Hello World, from Hy ITR!";
    return hello_world;
}

static std::shared_ptr<hyitr::TestThread> testTh;

void spawnThread()
{
    testTh = hyitr::ThreadManager::create<hyitr::TestThread>();
}

void stopThread()
{
    testTh->stop();
}
