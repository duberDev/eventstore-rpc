#!/bin/bash

# gem install -s http://rubygems.org/ grpc-tools

TOOLS_PATH=/c/RailsInstaller/Ruby2.2.0/lib/ruby/gems/2.2.0/gems/grpc-tools-1.1.2/bin/x86_64-windows

${TOOLS_PATH}/protoc.exe -I ../../protos --ruby_out=./ --grpc_out=./ --plugin=protoc-gen-grpc=${TOOLS_PATH}/grpc_ruby_plugin.exe ../../protos/event_store.proto
