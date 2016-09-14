XBUILD=xbuild
PACKAGE=NativeMemory

define run-xbuild
        $(XBUILD) $(PACKAGE).csproj /v:d $(if $(1),/t:$(1))
endef

APK = bin/Debug/$(PACKAGE)-Signed.apk

all:: $(APK)

clean::
	-$(RM) -rf obj bin

$(APK) : Makefile $(PACKAGE).csproj $(wildcard */*.cs *.cs)
	$(call run-xbuild,,$(INSTALL_EXTRA))

deploy:: $(APK)
	$(call run-xbuild,Install,$(INSTALL_EXTRA))

undeploy:: $(APK)
	$(call run-xbuild,Uninstall,$(UNINSTALL_EXTRA))

run::
	$(call run-xbuild,RunTests,$(RUNTESTS_EXTRA))
