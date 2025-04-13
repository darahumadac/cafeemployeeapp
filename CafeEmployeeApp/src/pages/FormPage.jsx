import React, {useEffect} from "react";
import axios from "axios";
import { useParams, useLocation } from "react-router-dom";
import { API_URL } from "../../config.js";



const FormPage = ({title, populate = false}) => 
{
    console.log(populate);
    const {pathname} = useLocation();

    useEffect(() => 
    {        
        populate && axios.get(`${API_URL}${pathname}`)
            .then((response) => {
                const data = response.data;
                console.log(data);
            }).catch(() =>  console.log("error loading"))
    },[]);

    const {id} = useParams();
    return (<div>{title} {id}</div>)

}

export default FormPage;