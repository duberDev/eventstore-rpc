package main

import (
	"google.golang.org/grpc"
	"golang.org/x/net/context"
	"github.com/satori/go.uuid"
	"fmt"
	"io"
	"bitbucket.org/jdextraze/eventstore-rpc/clients/go"
	"time"
	"log"
)

func main() {
	conn, err := grpc.Dial("192.168.99.100:3113", grpc.WithInsecure())
	if err != nil {
		panic(err)
	}
	defer conn.Close()

	esc := eventstore.NewEventStoreClient(conn)
	testAsync(esc)
}

func testAsync(esc eventstore.EventStoreClient) {
	res, err := esc.SubscribeToStreamFrom(context.Background())
	if err != nil {
		panic(err)
	}

	waitc := make(chan struct{})
	go func() {
		for {
			in, err := res.Recv()
			if err == io.EOF {
				close(waitc)
				return
			}
			if err != nil {
				log.Fatalf("Failed to receive a note : %v", err)
			}
			fmt.Println("<<<", in.String())
		}
	}()

	if err := res.Send(&eventstore.SubscribeToStreamFromRequest{StreamId: "Test"}); err != nil {
		panic(err)
	}

	go func() {
		for {
			res, err := esc.AppendToStream(context.Background(), &eventstore.AppendToStreamRequest{
				StreamId: "Test",
				ExpectedVersion: -2,
				Events: []*eventstore.EventData{
					{
						EventId: uuid.NewV4().Bytes(),
						EventType: "TestCreated",
						IsJson: true,
						Data: []byte(`{"test":"test"}`),
					},
				},
			})
			if err != nil {
				panic(err)
			}
			fmt.Println(">>>", res.String())
			time.Sleep(time.Second)
		}
	}()

	time.Sleep(time.Second * 10)

	if err := res.CloseSend(); err != nil {
		panic(err)
	}
	<-waitc
}