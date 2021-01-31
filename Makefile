SDK_IMAGE ?= mcr.microsoft.com/dotnet/sdk:5.0
TRAVIS_BUILD_NUMBER ?= dev

test:
	docker run --rm \
	  -v $(shell pwd):/app \
	  --workdir /app \
	  --entrypoint dotnet \
	  ${SDK_IMAGE} test
.PHONY: test

pack:
	docker run --rm \
	  -v $(shell pwd):/app \
	  --workdir /app \
	  --entrypoint dotnet \
	  ${SDK_IMAGE} pack \
	    -c Release \
	    --version-suffix alpha-${TRAVIS_BUILD_NUMBER} \
	    --output /app/package

	docker run --rm \
	  -v $(shell pwd):/app \
	  --workdir /app \
	  --entrypoint dotnet \
	  ${SDK_IMAGE} nuget push \
	    /app/package/MHWK.Serializer.1.*.nupkg \
	    --api-key ${NUGET_KEY} \
	    -s https://www.nuget.org/api/v2/package
.PHONY: pack
