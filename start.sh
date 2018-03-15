#!/bin/bash
export source_broker_list={source_broker_list}
export destination_broker_list={destination_broker_list}
export topics={topics}
export consumer_group={consumer_group}

cd DotnetKafkaMirror

dotnet restore
dotnet publish -o ./obj/Docker/publish

cd ../

docker build -t dotnet-kafka-mirror . --no-cache

docker run --env source_broker_list=$source_broker_list --env destination_broker_list=$destination_broker_list --env topics=$topics --env consumer_group=$consumer_group dotnet-kafka-mirror