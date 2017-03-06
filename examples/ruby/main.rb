this_dir = File.expand_path(File.dirname(__FILE__))
lib_dir = File.join(this_dir, 'lib')
$LOAD_PATH.unshift(lib_dir) unless $LOAD_PATH.include?(lib_dir)

require 'grpc'
require 'multi_json'
require 'event_store_services_pb'
require 'securerandom'

stub = Eventstore::EventStore::Stub.new('192.168.99.100:3113')
resp = stub.AppendToStream(Eventstore::AppendToStreamRequest.new(
    stream_id: 'Test',
    expected_version: -2,
    events: [
        Eventstore::EventData.new(
            event_id: SecureRandom::uuid(),
            event_type: 'TestCreated',
            is_json: true,
            data: '{"test":true}'
        )
    ]
))
puts resp.inspect
