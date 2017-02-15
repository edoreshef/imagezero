#include "libiz.h"
#include "image.h"

void __stdcall IZ_Init()
{
	IZ::initDecodeTable();
	IZ::initEncodeTable();
}

void __stdcall IZ_ReadHeader(unsigned char *src, int srcOffset, int& width, int& height, int& dataLength)
{
	// Decode image size
	IZ::Image<> img;
	img.setData(src + 4);
	IZ::decodeImageSize(img, src + srcOffset + 4);

	// Return data
	dataLength = ((int*)(src + srcOffset))[0];
	width = img.width();
	height = img.height();
}

void __stdcall IZ_Decode(unsigned char *src, int srcOffset, unsigned char *dst)
{
	IZ::Image<> img;
	img.setData(dst);
	IZ::decodeImageSize(img, src + srcOffset + 4);
	IZ::decodeImage(img, src + srcOffset + 4);
}

int __stdcall IZ_Encode(unsigned char *src, int imgWidth, int imgHeight, unsigned char *dst, int dstOffset)
{
	IZ::Image<> img;
	img.setWidth(imgWidth);
	img.setHeight(imgHeight);
	img.setSamplesPerLine(imgWidth * 3);
	img.setData(src);
	unsigned char *destEnd = IZ::encodeImage(img, dst + dstOffset + 4);

	// Compute and return size
	return ((int*)(dst + dstOffset))[0] = destEnd - dst;
}
