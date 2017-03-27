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
	conn, err := grpc.Dial("192.168.99.100:3113", grpc.WithInsecure(), grpc.WithBlock())
	if err != nil {
		panic(err)
	}
	defer conn.Close()

	testAsync(conn, fmt.Sprintf("Test-%s", uuid.NewV4()))
}

func testAsync(conn *grpc.ClientConn, streamId string) {
	esc := eventstore.NewEventStoreClient(conn)
	ctx := context.Background()
	res, err := esc.SubscribeToStreamFrom(ctx)
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
			fmt.Printf("%s <<< %s\n", time.Now(), in)
		}
	}()

	if err := res.Send(&eventstore.SubscribeToStreamFromRequest{StreamId: streamId}); err != nil {
		panic(err)
	}

	go func() {
		for i := 0; i < 8 ; i++ {
			res, err := esc.AppendToStream(ctx, &eventstore.AppendToStreamRequest{
				StreamId: streamId,
				ExpectedVersion: -2, //int32(i - 1),
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
			fmt.Printf("%s >>> %s\n", time.Now(), res)
			time.Sleep(time.Second)
		}
	}()

	time.Sleep(time.Second * 30)
	//for i := 0; i < 30; i++ {
	//	if err := res.Send(&eventstore.SubscribeToStreamFromRequest{StreamId: streamId}); err != nil {
	//		panic(err)
	//	}
	//	time.Sleep(time.Second)
	//}

	if err := res.CloseSend(); err != nil {
		panic(err)
	}
	<-waitc
}