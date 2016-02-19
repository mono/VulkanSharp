VK_XML_URL = https://raw.githubusercontent.com/KhronosGroup/Vulkan-Docs/1.0/src/spec/vk.xml

CONFIGURATION = Debug
BIN_PATH = bin/$(CONFIGURATION)

all: $(BIN_PATH)/vk.xml
	xbuild

$(BIN_PATH)/vk.xml:
	curl -o "$@" $(VK_XML_URL)

clean:
	rm -Rf $(BIN_PATH)
