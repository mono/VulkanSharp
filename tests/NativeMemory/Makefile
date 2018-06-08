MSBUILD=msbuild
PACKAGE=NativeMemory

define run-msbuild
        $(MSBUILD) $(PACKAGE).csproj /v:d $(if $(1),/t:$(1))
endef

APK = bin/Debug/$(PACKAGE)-Signed.apk

all:: $(APK)

clean::
	-$(RM) -rf obj bin

$(APK) : Makefile $(PACKAGE).csproj $(wildcard */*.cs *.cs)
	$(call run-msbuild,,$(INSTALL_EXTRA))

deploy:: $(APK)
	$(call run-msbuild,Install,$(INSTALL_EXTRA))

undeploy:: $(APK)
	$(call run-msbuild,Uninstall,$(UNINSTALL_EXTRA))

run::
	$(call run-msbuild,RunTests,$(RUNTESTS_EXTRA))
