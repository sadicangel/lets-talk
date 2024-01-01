export function getCookie(name: string) {
    const cookieName = name + "=";
    const decodedCookie = decodeURIComponent(document.cookie);
    const cookies = decodedCookie.split(';');
    for (const cookie of cookies) {
        let i = 0;
        while (cookie.charAt(i) == ' ') {
            ++i;
        }
        const trimmedCookie = cookie.substring(i);
        if (trimmedCookie.indexOf(cookieName) == 0) {
            return trimmedCookie.substring(cookieName.length, trimmedCookie.length);
        }
    }
    return "";
}


export function setCookie(name: string, value: string, exdays: number, secure: boolean) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    if (secure) {
        document.cookie = name + "=" + value + ";" + expires + ";path=/; secure";
    } else {
        document.cookie = name + "=" + value + ";" + expires + ";path=/";
    }
}


export function deleteCookie(name: string) {
    document.cookie = name + '=; Max-Age=-99999999;';
}