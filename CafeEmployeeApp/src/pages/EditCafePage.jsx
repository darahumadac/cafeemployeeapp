import React, {useEffect, useState} from "react";
import { Container } from "@mui/material";
import Table from "../Table";
import axios from "axios";
import { useParams } from "react-router-dom";



const EditCafePage = () => 
{
    const {id} = useParams();
    return (<div>edit cafe {id}</div>)

}

export default EditCafePage;