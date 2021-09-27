DIRS = \
	app \
	lib-v1 \
	lib-v2

all:
	@for d in $(DIRS) ; do dotnet build $$d ; done

run:
	dotnet app/bin/Debug/net6.0/multi-versions.dll lib-v1/bin/Debug/net6.0/lib.dll lib-v2/bin/Debug/net6.0/lib.dll
