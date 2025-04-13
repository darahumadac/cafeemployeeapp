import { AgGridReact } from 'ag-grid-react';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';


const Table = ({rowData, columnDefs}) =>
{
    return(
        <AgGridReact
        rowData={rowData}
        columnDefs={columnDefs}
        pagination={true}
         
        
    />
    );
    
}

export default Table;