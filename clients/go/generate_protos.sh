#!/bin/bash

TOOLS_PATH=../../server/packages/Grpc.Tools.1.1.0/tools/windows_x86

${TOOLS_PATH}/protoc.exe -I ../../protos ../../protos/event_store.proto --go_out=plugins=grpc:./
