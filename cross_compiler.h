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
	/* n が0のときの __builtin_clz の戻り値は未定義なので、
	 * _BitScanReverse の戻り値チェックは割愛する。
	 */
	_BitScanReverse(&index, n);
	return 31 - index;
}
 
static ALWAYS_INLINE
int __builtin_ctz(unsigned int n)
{
	unsigned long index;
	/* n が0のときの __builtin_ctz の戻り値は未定義なので、
	 * _BitScanForward の戻り値チェックは割愛する。
	 */
	_BitScanForward(&index, n);
	return index;
}

#elif defined(__GNUC__)

#define ALWAYS_INLINE inline __attribute__((__always_inline__)

#endif
