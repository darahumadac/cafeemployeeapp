import React, {useEffect, useState} from "react";
import { Container } from "@mui/material";
import Table from "../Table";
import axios from "axios";



const CafesPage = () => 
{
    const [columns, setColumns] = useState([
        { field: 'name'},
        { field: 'description' },
        { field: 'employees' },
        { field: 'location' },
        { field: 'edit' },
        { field: 'delete' }
    ]);
    const [rows, setRows] = useState([]);

    useEffect(() => 
    {
        axios.get("http://localhost:5191/cafes")
            .then((response) => {
                const cafeList = response.data;
                setRows(cafeList);
          });
    },[]);

    // const columnDefs = [
    //     { field: 'name', sortable: true, filter: true },
    //     { field: 'age' },
    //     { field: 'country' }
    //   ];
    
    //   const rowData = [
    //     { name: 'Alice', age: 25, country: 'USA' },
    //     { name: 'Bob', age: 30, country: 'UK' }
    //   ];
    
      return (
        // <div className="ag-theme-alpine">
        <Container maxWidth style={{height: 400}}>
           <Table columnDefs={columns} rowData={rows}/>
        </Container>
          
        // </div>
      );

}

export default CafesPage;