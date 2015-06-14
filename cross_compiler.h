#pragma once

// http://blog.jiubao.org/2015/01/gcc-bitscanforward-bitscanreverse-msvc.html
// http://homepage1.nifty.com/herumi/prog/gcc-and-vc.html


#ifdef _MSC_VER

#include <intrin.h>

#define ALWAYS_INLINE __forceinline

static ALWAYS_INLINE
int __builtin_clz(unsigned int n)
{
	unsigned long index;
	/* n ��0�̂Ƃ��� __builtin_clz �̖߂�l�͖���`�Ȃ̂ŁA
	 * _BitScanReverse �̖߂�l�`�F�b�N�͊�������B
	 */
	_BitScanReverse(&index, n);
	return 31 - index;
}
 
static ALWAYS_INLINE
int __builtin_ctz(unsigned int n)
{
	unsigned long index;
	/* n ��0�̂Ƃ��� __builtin_ctz �̖߂�l�͖���`�Ȃ̂ŁA
	 * _BitScanForward �̖߂�l�`�F�b�N�͊�������B
	 */
	_BitScanForward(&index, n);
	return index;
}

#elif defined(__GNUC__)

#define ALWAYS_INLINE inline __attribute__((__always_inline__)

#endif
