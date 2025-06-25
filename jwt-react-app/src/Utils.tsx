export const setAccessToken = (token: string) => {
    localStorage.setItem('AccessToken', token);
};


export const removeAccessToken = () => {
    localStorage.removeItem('AccessToken');
};

export const setRefreshToken = (token: string) => {
    localStorage.setItem('RefreshToken', token);
};

export const removeRefreshToken = () => {
    localStorage.removeItem('RefreshToken');
};

export const getRefreshToken = (): string => {
    return localStorage.getItem('RefreshToken') || "";
}
export const getAccessToken = (): string => {
    return localStorage.getItem('AccessToken') || "";
};

export const Inspect = (funName: string, data: any) => {
    if (data instanceof Array) {
        let items = "";
        data.forEach((element, index) => {
            items += `${index + 1}:${element}\n`
        });
        console.log(`called from ${funName} => ${items}`)
    }
    else {
        console.log(`called from ${funName} => ${data}`)
    }

}